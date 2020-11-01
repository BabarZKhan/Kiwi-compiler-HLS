// starter-code
// SoC System Integrator

type desmodule_purpose_t =
    | DF_primary
    | DF_subsidiary
    | DF_concentrator
    | DF_zonebridge
    | DF_static_port    
    | DF_adaptor

let purposeToStr x = sprintf "%A" x


let is_nailed = function // Cannot be moved between zones
    | DF_zonebridge
    | DF_static_port    -> true
    | _ -> false
    
type desport_purpose_t =
    | DP_var of string            // Available for unification
    | DP_named_group of string    // A grounded constant
    | DP_multi                    // Outside of the binding/unification process.
    | DP_filler    

let rec domToStr env = function
    | DP_var vv ->
        match op_assoc vv env with
            | None    -> vv
            | Some ss ->  sprintf "%s===%s" vv (domToStr env ss)
    | DP_named_group ss -> ss
    | other -> sprintf "%A" other

type unit_of_soc_measure_t =
  {
      name:  string
      units: string
      value: double
      sumrule: string
  }


let g_default_area_uom =       { name="Area"; value=0.0; units="NAND2-equivalents"; sumrule="zoned_sum" }
let g_default_latency_uom =    { name="Latency"; value=1.0; units="Clock Cycles"; sumrule="sum" }
let g_default_bandwidth_uom =  { name="Bandwidth"; value=1.0; units="bps"; sumrule="sum" }


type layout_zone_name_t = string

type busbabs_t =
    {
        onetoonef:      bool
        vlnv:           ip_xact_vlnv_t 
        portmeta:       (string*string) list
    }
    
type desmodule_port_t =
  {
      dp_nailed:           layout_zone_name_t option
      dp_name:             string  // Port name
      busbabs:             busbabs_t
      masterf:             bool                  // Holds if this port is a master (not slave).
      bandwidth:           unit_of_soc_measure_t
      domain:              desport_purpose_t     // Group or domain
      dp_needs_connection: bool
  }


let gec_busbabs vlnv = { onetoonef=true; vlnv=vlnv; portmeta=[] }

let g_default_desmodule_port =
  {
      dp_nailed=           None
      dp_name=             "-anon-"
      busbabs=             gec_busbabs { vendor="hpr"; library="ip0"; kind="-anon-"; version="1.0" }
      masterf=             false  // Holds if this port is a master (not slave).
      bandwidth=           g_default_bandwidth_uom
      domain=     DP_filler
      dp_needs_connection= true    // Only set this false for additional ports on a concentrator/aggregator or static ports
  }

type desmodule_t =
  {
    name:              ip_xact_vlnv_t 
    area:              unit_of_soc_measure_t
    latency:           unit_of_soc_measure_t    
    form:              desmodule_purpose_t
    ports:             desmodule_port_t list
  }



type desmodule_instance_t =
  {
    iname:             string
    kind:              desmodule_t
    nailed_:           layout_zone_name_t option // implies all ports are nailed to this zone too
  }



type soclayout_zone_t = // Normally this is one FPGA
  {
      name:              layout_zone_name_t
      area:              unit_of_soc_measure_t
      connected_peers:   string list
      static_contents:   desmodule_instance_t list
  }

type block_placement_t =
    {
        where:  soclayout_zone_t
        what:   desmodule_instance_t
    }

type soc_design_t =
    | DES_empty
    | DES_augment of string * block_placement_t * soc_design_t
    

type conquad_t = string * string * layout_zone_name_t option * desmodule_port_t // Fields are (iname, common iname for a bridge, zone option, port)




// ------------------------------------------------------------
   
let example_static_axi_m =
  {
    name=              { vendor="hpr"; library="ip0"; kind="axi4-static-master101"; version="1.0" }
    area=              { g_default_area_uom with value=4020.1; }
    latency=           { g_default_latency_uom with value=2.0; }    
    form=              DF_static_port 
    ports=             [
                         { g_default_desmodule_port with
                             dp_name=         "axi4-static-master101p0"
                             busbabs=         gec_busbabs { vendor="hpr"; library="ip0"; kind="axi4-D32"; version="1.0" }
                             masterf=         true  // Holds if this port is a master (not slave).
                             bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                             domain= DP_named_group "directing"
                             dp_needs_connection= false
                         }

                       ] 
  }

let example_static_axi_s =
  {
    name=              { vendor="hpr"; library="ip0"; kind="axi4-static-slave202"; version="1.0" }
    area=              { g_default_area_uom with value=4020.1; }
    latency=           { g_default_latency_uom with value=2.0; }    
    form=              DF_static_port 
    ports=             [
                         { g_default_desmodule_port with 
                             dp_name=         "axi4-static-slave202p0"
                             busbabs=         gec_busbabs { vendor="hpr"; library="ip0"; kind="axi4-D32"; version="1.0" }
                             masterf=         false  // Holds if this port is a master (not slave).
                             bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                             domain= DP_named_group "memoryservice" 
                             dp_needs_connection= false
                         }

                       ] 
  }


let example_axi_director_shim = // block for the demo library: mailbox paradigm but is classed as subsidiary (adaptors must have one master and one slave port).
  {
    name=              { vendor="hpr"; library="ip0"; kind="axi_dir_shim"; version="1.0" }
    area=              { g_default_area_uom with value=800.1; }
    latency=           { g_default_latency_uom with value=2.0; }    
    form=              DF_subsidiary // a mailbox style subsidiary block
    ports=             [
                         { g_default_desmodule_port with 
                             dp_name=         "dirshim-axi4-slave-port"
                            
                             busbabs=         gec_busbabs { vendor="hpr"; library="ip0"; kind="axi4-D32"; version="1.0" }
                             masterf=         false  // Holds if this port is a master (not slave).
                             bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                             domain= DP_named_group "directing"
                         };
                         { g_default_desmodule_port with 
                             dp_name=         "dirshim-directorate12-port"
                             busbabs=         gec_busbabs { vendor="hpr"; library="ip0"; kind="directorate12"; version="1.0" }
                             masterf=         false  // Holds if this port is a master (not slave).
                             bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                             domain= DP_named_group "directing"
                         }
                       ]

  }
  


// Hardcoded instances for testing
let axi_m1 =
    {
        iname= "axi_m1"
        kind= example_static_axi_m
        nailed_=  Some "chip1"
    }

let axi_m2 =
    {
        iname= "axi_m2"
        kind= example_static_axi_m
        nailed_=  Some "chip1"
    }

let axi_directorate_slave =
    {
        iname=   "axi_s1"
        kind=    example_static_axi_s
        nailed_=  Some "chip1"
    }

let axi_directorate_master =
    {
        iname=   "axi_m1"
        kind=    example_static_axi_m
        nailed_=  None
    }


// Concentrators applied to static ports do not need to be available in pairs, since the `other' side is not under our control.
let gec_example_concentrators _ =
                    
    let gec_canned_concentrator protocol arity side = // Side holds for the MUX, which has a master focus port.

        let gec_conc_port no =
          { g_default_desmodule_port with 
              dp_name=         "childport" + i2s no
              busbabs=         protocol
              masterf=         not side // Holds if this port is a master (not slave).
              bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
              domain=          DP_multi
              dp_needs_connection= false
           }

        let focus =
          { g_default_desmodule_port with 
              dp_name=         "focus"
              busbabs=         protocol
              masterf=         side  // For a MUX the focus is a master. Focus is a slave on a DEMUX.
              bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
              domain=          DP_multi
          }

        let ans = 
          {
            name=              { vendor="hpr"; library="canned-examples"; kind=sprintf "concen_%i_%s_%s" arity protocol.vlnv.kind (if side then "MUX" else "DEMUX"); version="1.0" }
            area=              { g_default_area_uom with value=25.2 * (double)arity; }
            latency=           { g_default_latency_uom with value=2.0; }    
            form=              DF_concentrator
            ports=             focus :: map gec_conc_port [0..arity-1]
          }
        ans

    // Muxes and demuxes must (currently) be available in the same matching arity sets.
    let gec_canned_concentrator_pair protocol arity = [ gec_canned_concentrator protocol arity true; gec_canned_concentrator protocol arity false; ] 
    let ans =
        [ gec_canned_concentrator_pair (gec_busbabs { vendor="hpr"; library="canned-protocols"; kind="axi4-D32"; version="1.0" }) 2
          gec_canned_concentrator_pair (gec_busbabs { vendor="hpr"; library="canned-protocols"; kind="axi4-D32"; version="1.0" }) 4
        ]

    let normalise conc = conc // We must ensure the focus port is first - clearly it is while we are canned!

    map normalise (list_flatten ans)

        
let gec_example_adaptors _ =
    let sorter ports =
        let masters = List.filter (fun port->port.masterf) ports
        masters @ list_subtract(ports, masters)

    let gen_canned_adaptor0 flip tag =
      let dom = DP_var(funique "AZ1")   // Use Prolog-style capitals for variables yet to be bound // Needs freshening on each retrieve
      {
        name=              { vendor="hpr"; library="canned-examples"; kind="axi4-32_axi3-32" + tag ; version="1.0" }
        area=              { g_default_area_uom with value=10.3; }
        latency=           { g_default_latency_uom with value=2.0; }    
        form=              DF_adaptor
        ports= sorter      [
                             {g_default_desmodule_port with 
                                 dp_name=         "port-left"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi3-D32"; version="1.0" }
                                 masterf=         flip // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=12.1;  }
                                 domain=          dom // We could brand this as  "directing" here but thats implicit from the busbabs kind.
                             }
                             {  g_default_desmodule_port with 
                                 dp_name=         "port-right"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-D32"; version="1.0" }
                                 masterf=         not flip  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=21.1;  }
                                 domain=          dom
                             }
                           ] 
      }

    let gen_canned_adaptor1 flip tag = 
      let dom = DP_var(funique "Gsos")   // Use Prolog-style capitals for variables yet to be bound.
      {
        name=              { vendor="hpr"; library="canned-examples"; kind="axi4_axi4_lite_adaptor_D32_" + tag ; version="1.0" }
        area=              { g_default_area_uom with value=40.3; }
        latency=           { g_default_latency_uom with value=2.0; }    
        form=              DF_adaptor
        ports= sorter      [
                             {g_default_desmodule_port with 
                                 dp_name=         "port-left"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-lite-D32"; version="1.0" }
                                 masterf=         flip // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom // We could brand this as  "directing" here but thats implicit from the busbabs kind.
                             }
                             {  g_default_desmodule_port with 
                                 dp_name=         "port-right"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-D32"; version="1.0" }
                                 masterf=         not flip  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                           ] 
      }

// Post hoc optimise perhaps needed where a pair of inverse adaptors are back-to-backed
// There is a shim to use instead?
    let gen_canned_adaptor2 flip tag =
      let dom = DP_var(funique "Gcar")   // Use Prolog-style capitals for variables yet to be bound        
      {
        name=              { vendor="hpr"; library="canned-examples"; kind="adaptor-loadstore10-axi_" + tag ; version="1.0" }
        area=              { g_default_area_uom with value=40.3; }
        latency=           { g_default_latency_uom with value=2.0; }    
        form=              DF_adaptor

        ports= sorter      [
                             { g_default_desmodule_port with 
                                 dp_name=         "port-left"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-D32"; version="1.0" }
                                 masterf=         flip // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                             { g_default_desmodule_port with 
                                 dp_name=         "port-right"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="loadstore10"; version="1.0" }
                                 masterf=         not flip  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                           ] 
      }

// Post hoc optimise perhaps needed where a pair of inverse adaptors are back-to-backed
// There is a shim to use instead?
    let gen_canned_adaptor3 flip tag =
      let dom = DP_var(funique "Gcar")   // Use Prolog-style capitals for variables yet to be bound        
      {
        name=              { vendor="hpr"; library="canned-examples"; kind="adaptor-directorate12-axi_" + tag ; version="1.0" }
        area=              { g_default_area_uom with value=40.3; }
        latency=           { g_default_latency_uom with value=2.0; }    
        form=              DF_adaptor

        ports= sorter      [
                             { g_default_desmodule_port with 
                                 dp_name=         "port-axi4"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-D32"; version="1.0" }
                                 masterf=         flip // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                             { g_default_desmodule_port with 
                                 dp_name=         "port-dir12"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="directorate12"; version="1.0" }
                                 masterf=         not flip  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                           ] 
      }


    [gen_canned_adaptor0 false "zunter"; gen_canned_adaptor0 true "zuber" ] @
    [gen_canned_adaptor1 false "unter"; gen_canned_adaptor1 true "uber" ]  @
    [gen_canned_adaptor2 false "punter"; gen_canned_adaptor2 true "puber" ] @
    [gen_canned_adaptor3 false "funter"; gen_canned_adaptor3 true "fuber" ]

    
let make_example_bridge name (f_zone, t_zone) =
    let vv = funique ("Bdom") // Use Prolog-style capitals for variables yet to be bound

    // A bridge is formed of a pair of ramps, one in each zone.
    let gen_canned_ramp masterf zone = //
      {
        name=              { vendor="hpr"; library="canned-examples"; kind=name + "-ramp" + (if masterf then "M" else "S"); version="1.0" }
        area=              { g_default_area_uom with value=40.3;  } // Area in each zone needed for a bridge
        latency=           { g_default_latency_uom with value=2.0; }    
        form=              DF_zonebridge // halfbridge
        ports=             [
                             { g_default_desmodule_port with
                                 dp_name=         if masterf then "PortM" else "PortS"
                                 dp_nailed=       Some zone
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-D32"; version="1.0" }
                                 masterf=         masterf  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain= DP_var vv
                                 dp_needs_connection= false  // Bridges in the manifest do not need to be used. If one half is used then the other half needs using but that is done constructively at bridge deployment.
                             }
                           ] 
      }
    // Simplex initiating bridge
    let ramp_gen iname masterf zone = 
        {
          iname=       iname                          // CRITICALLY we have the same instance name and group name at both ends
          kind=        gen_canned_ramp (masterf) zone // But different direction at each end
          nailed_=      None
        }
    ((f_zone, t_zone), name, (ramp_gen (name + "launch") false f_zone, ramp_gen (name + "landing") true  t_zone))


// HPR System_Integrator: Cannot find any registered subsidiary IP block to instantiate so as to serve Master:*top-primary-IP-block*/loadstore10m-port/loadstore10/1.0
// Adapting route finding ...
// The offchip service adaptor should connect to a static memory sevice domain named port.  We can then adapt to it.

// Adaptors in pairs?
// Dont need this one since an adaptor will serve - but currently subsidiary needs cannot be met with an adaptor .... please rationalise and write up
let loadstore10_axi_shim = // We term an adaptor for subsidiary use a 'shim'

      let dom = DP_var(funique "Gshi")   // Use Prolog-style capitals for variables yet to be bound
      {
        name=              { vendor="hpr"; library="canned-examples"; kind="offchip-memory-service-shimr"; version="1.0" }
        area=              { g_default_area_uom with value=140.3;  }
        latency=           { g_default_latency_uom with value=2.0; }    
        form=              DF_subsidiary
        ports=             [
                             { g_default_desmodule_port with
                                 dp_name=         "offchip-memory-service-port"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="loadstore10"; version="1.0" }
                                 masterf=         false  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                             { g_default_desmodule_port with
                                 dp_name=         "axiout"
                                 busbabs=         gec_busbabs { vendor="hpr"; library="canned-examples"; kind="axi4-D32"; version="1.0" }
                                 masterf=         true  // Holds if this port is a master (not slave).
                                 bandwidth=       { g_default_bandwidth_uom with value=30.1;  }
                                 domain=          dom
                             }
                           ] 
      }


// Discuss mailbox paradigm
    
let example_primary_block = //  desmodule_t 

    let pp1 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "dir12port"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="directorate12"; version="1.0" }
          masterf=           true  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=33.1;  }
          domain=   DP_named_group "directing" //  desport_purpose_t
      }

    let pp2 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "bram33port"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="bram33"; version="1.0" }
          masterf=           true  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=233.1;  }
          domain=   DP_named_group "memoryservice" //  desport_purpose_t
      }

    let pp3 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "loadstore10m-port"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="loadstore10"; version="1.0" }
          masterf=           true  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=233.1;  }
          domain=   DP_named_group "memoryservice" //  desport_purpose_t
      }

    let pp4 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "subsa0-master-port0"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="subsa0"; version="1.0" }
          masterf=           true  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=233.1;  }
          domain=   DP_named_group "memoryservice" //  desport_purpose_t
      }

    {
            name=     { vendor="hpr"; library="des0"; kind="primex"; version="1.0" }
            area=     { g_default_area_uom with value=844.1; }
            latency=           { g_default_latency_uom with value=2.0; }    
            form=     DF_primary
            ports=    [ pp1;  pp2; pp3; pp4 ]

    }
    
let example_subs_block = //  desmodule_t 

    let pp1 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "subsa0-slave-port"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="subsa0"; version="1.0" }
          masterf=           false  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=33.1;  }
          domain=   DP_var "SubsVar0"
      }

    let pp2 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "loadstore10-m-port"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="loadstore10"; version="1.0" }
          masterf=           true  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=233.1;  }
          domain=   DP_named_group "memoryservice" //  desport_purpose_t
      }

    {
            name=     { vendor="hpr"; library="des0"; kind="primsubs55"; version="1.0" }
            area=     { g_default_area_uom with value=844.1; }
            latency=           { g_default_latency_uom with value=2.0; }    
            form=     DF_primary
            ports=    [ pp1;  pp2 ]

    }

let example_ram44 = //  desmodule_t 

    let pp1 = // desmodule_port_t
      { g_default_desmodule_port with
          dp_name=           "BRAM-slave-port"
          busbabs=           gec_busbabs { vendor="hpr"; library="des0"; kind="bram33"; version="1.0" }
          masterf=           false  // Holds if this port is a master (not slave).
          bandwidth=         { g_default_bandwidth_uom with value=33.1; }
          domain=   DP_named_group "memoryservice" //  General RAM resources go in the group 'memoryservice' by default
      }


    {
            name=     { vendor="hpr"; library="des0"; kind="example_BRAM44"; version="1.0" }
            area=     { g_default_area_uom with value=2814.0; }
            latency=  { g_default_latency_uom with value=2.0; }    
            form=     DF_subsidiary
            ports=    [ pp1; ]

    }


let g_temporary_block_database = ref [] // of desmodule_t
let g_temporary_bridge_instance_database = ref []
let g_temporary_concentrator_instance_database = ref []
let g_adaptor_database = ListStore<string * string, desmodule_t>("adaptor_database")

let t100 = make_example_bridge "ONE-to-TWO"  ("chip1", "chip2")
let t200 = make_example_bridge "TWO-to-ONE"  ("chip2", "chip1")

let list_component_once lst =
    let rec onelister sofar = function
        | [] -> []
        | a::tt ->
            if memberp a.iname sofar then onelister sofar tt else a :: onelister (a.iname :: sofar) tt

    onelister [] lst

let temporaryloadlib() =
    let _ = 
        let install (block:desmodule_t) = mutadd g_temporary_block_database (block.name, block)
        let items = [ example_primary_block; example_subs_block;  example_ram44 ]  @ [  example_axi_director_shim; loadstore10_axi_shim ] 
        app install items

    let _ =
        let install x =
            mutadd g_temporary_bridge_instance_database x
        app install ((* list_component_once *) [t100 ; t200])

    let _ =
        let install x =
            mutadd g_temporary_concentrator_instance_database x
        app install (gec_example_concentrators())

    let _ =
        let install (block:desmodule_t) =
            let _ = if length block.ports <> 2 then sf (sprintf "Expected two ports on protocol adaptor called " + block.name.kind)
            let l = (hd block.ports).busbabs.vlnv.kind
            let r = (cadr block.ports).busbabs.vlnv.kind
            let _ = vprintln 0 (sprintf "   add adapator to library  l=%s   r=%s"  l r)
            g_adaptor_database.add (l, r) block
            // Should add these to master block_database too really
        let items = gec_example_adaptors ()  
        app install items
    ()
    
let example_chip1 =
  {
      name=              "chip1"
      area=              { g_default_area_uom with value=80000.1; }
      connected_peers=   [ "chip2"; ]
      static_contents=   [ axi_m1; axi_m2; axi_directorate_slave; ] // @ t100
  }

let example_chip2 =
  {
      name=              "chip2"
      area=              { g_default_area_uom with value=80000.1; }
      connected_peers=   [ "chip1"; ]
      static_contents=   [ ] // @ t200
  }



// Dictionary of available layout resources (zones).
let g_soclayout_zone_database = OptionStore<string, soclayout_zone_t>("layoutzone_database")
let g_soclayout_zone_list = ref []
// System Instantiator Makes an Instance of the top-level component and all of the
// support it needs.


let install_layout_resource ww (chip:soclayout_zone_t) =
    mutadd g_soclayout_zone_list chip
    g_soclayout_zone_database.add chip.name chip
    ()

type p_concs_sortedout_t = OptionStore<string, (int * desmodule_t list * desmodule_t list) list>

type soc_inst_settings_t =
    {
        banner:          string
        table_reports:   string list ref
        p_concs_sortedout:  p_concs_sortedout_t
        prng:               int ref
        dlimit:             int  // Diameter of connections - set low for debugging complexity explosions, otherwise can be high for normal operation.
        render_dot:         bool  // Whether to write output graphically
        render_rtl:         bool  // Whether to write output as an RTL structural netlist.
        render_ip_xact:     bool  // Whether to write output as an IP-XACT design        
    }

let uom_report_full (uom:unit_of_soc_measure_t) =
    sprintf "%s %f %s" uom.name uom.value uom.units

let uom_report (uom:unit_of_soc_measure_t) =
    sprintf "%f %s" uom.value uom.units

let uom_zero (uom:unit_of_soc_measure_t) = { uom with value=0.0 }

let uom_sum (lhs:unit_of_soc_measure_t) (rhs:unit_of_soc_measure_t) =
    if (lhs.units <> rhs.units) then sf (sprintf "uom_sum: cannot add apples to oranges: %s cf %s" (uom_report_full lhs) (uom_report_full rhs))
    else

        // need to dispatch over sumfun
        { lhs with value= lhs.value+rhs.value }

let prng settings =
    let v = !settings.prng
    let v = if v = 0 then 111111111 else v
    let a = 16807      // Park-Miller uses 7**5
    let v = (a * v) &&& 0x7FFFffff
    let _ = settings.prng := v
    v
    
let uom_dgtd (lhs:unit_of_soc_measure_t) (rhs:unit_of_soc_measure_t) =
    if (lhs.units <> rhs.units) then sf (sprintf "uom_dgtd: cannot compare apples to oranges: %s cf %s" (uom_report_full lhs) (uom_report_full rhs))
    else lhs.value > rhs.value 


let temp_block_assoc ww msg names =
    match List.filter (fun (_, x)->(x:desmodule_t).name.kind=names) !g_temporary_block_database with // Need vlnv lookup
       | []      -> sf (msg + sprintf ": cannot find IP block definition named '%s'" (names))
       | [ (_, v) ] ->v
        | items -> sf (msg + sprintf ": Found multiple find IP block definitions named '%s'.\n Answers are=\n%s" (names) (sfoldcr_lite (fun (_, x)->vlnvToStr (x:desmodule_t).name) items))
        

// We always freshen domain variable in returned instance (to allow unification for different applictions)
let find_adaptor_block_serf ww msg init_side targ_side = // initiating left to right - left port should be bindable to a master and so is itself a slave
    match g_adaptor_database.lookup (init_side, targ_side) with
        | [] ->
            cleanexit(msg + sprintf ": Cannot find bus adaptor between %s and %s " (init_side) (targ_side)) // TODO report portmeta mismatch too
        | s::_ -> // Select on best TODO
            // TODO -here report on version/lib discrepancies - no that is now done in unifier ...
            // TODO check for is DF_adaptor
            if length s.ports <> 2 then cleanexit(msg + ": adaptor should be defined with precisely 2 ports: " + vlnvToStr s.name)
            let (p1, p2) = (hd s.ports, cadr s.ports)
            let (master_port, slave_port) =
                match (p1.masterf, p2.masterf) with
                    | (true, false) -> (p1, p2)
                    | (false, true) -> (p2, p1)
                    | _ -> cleanexit(msg + ": adaptor should be defined with one master (initiating) and one slave (target) port: " + vlnvToStr s.name)
            let freshened = DP_var(funique "Adaptorx")
            (s, { slave_port with domain=freshened }, { master_port with domain=freshened })

let find_adaptor_block ww msg port_s port_m = // initiating left to right - left port should be bindable to a master and so is itself a slave
    let (init_side, targ_side) = (port_m.busbabs.vlnv.kind, port_s.busbabs.vlnv.kind)
    find_adaptor_block_serf ww msg  init_side targ_side
    

let portmeta_mismatched msg domain_unifications lp rp = // Return info string and mismatch severity.  100 or above is fatal mismatch. Less than 100 may well work at user's risk
    let (l, r) = (lp.busbabs, rp.busbabs)
    let (msg, severity) =
        if l.vlnv.kind <> r.vlnv.kind then
            (sprintf "ProtocolMismatched! l=%s r=%s" l.vlnv.kind r.vlnv.kind, 1000)
        elif l.onetoonef <> r.onetoonef then
            (sprintf "ProtocolArityMismatched!  l=%s r=%s" l.vlnv.kind r.vlnv.kind, 1000) // Should not happen

        elif l.vlnv <> r.vlnv then
            (sprintf "ProtocolMinorMismatch  l=%s r=%s" (vlnvToStr l.vlnv) (vlnvToStr r.vlnv), 10)
            // TODO check portmeta here - e.g. bus widths

        else ("ok", 0)

    let unify du l r =
        let is_const = function
            | DP_named_group _ -> true
            | _                -> false

        let has_const lst =
            let dpred = function
                | DP_named_group _ -> true
                | _                -> false
            disjunctionate dpred lst

        let rec u_assoc xx = function
            | []                      -> None
            | h::tt when memberp xx h -> Some h
            | _::tt                   -> u_assoc xx tt

        let augment blk newmember du =
            let rec augf sofar = function
                | [] -> [newmember] :: du
                | h::tt when h=blk -> (rev sofar) @ (singly_add newmember h) :: tt
                | h::tt -> augf (h::sofar) tt
            augf [] du

        let conglom lb rb du = (lst_union lb rb) :: list_subtract(du, [lb;rb])
            
        if l=DP_multi || r=DP_multi || l=r then (true, du)
        else
            // At most one DP_named_group is allowed in any unification group
            match (u_assoc l du, u_assoc r du) with
                | (None, None)    -> if is_const l && is_const r then (false, du) else (true, [l; r] :: du)
                | (Some lb, None) -> if is_const r && has_const lb then (false, du) else (true, augment lb r du)
                | (None, Some rb) -> if is_const l && has_const rb then (false, du) else (true, augment rb l du)
                | (Some lb, Some rb) -> if lb=rb then (true, du) elif is_const l && is_const r then (false, du) else (true, conglom lb rb du)

    if (severity >= 100) then (msg, severity, domain_unifications)
    else
        let (okf, domain_unifications)  = unify domain_unifications lp.domain rp.domain
        if okf then (msg, severity, domain_unifications)
        else ("unification failed", 1000, domain_unifications)


let consort quads  = // Put initiating conquad first on a link.  (For a component we want the target port first to maintain left-to-right initiation flow).
    let (masters, slaves) = groom2 (fun quad->(f4o4 quad).masterf) quads
    masters @ slaves
//
// Recursively add primary and secondary blocks until all port domains are satisfied. 
// Services provided by static ports may need adaptors...
// Instantiatng a BRAM and connecting to external DRAM are both potentially feasible, but will differ in cost preference metric.        
// An adaptor has the same domain name on each side but differs in protocol (aka busbabs).
// 
let system_integrator_block_port_closer ww cX static_ports iteration domain_unifications blockset =
    let (settings, collated_concentrators_by_protocol, zonewiring, adaptor_info) = cX
    let (protocol_names, interprotocol_matrix, infinity, protocol_idxof, rev_protocol_idxof) = adaptor_info
    let msg = settings.banner
    let form = "subsidiary"

    let collect_instance_ports cc (pf_, instance) = // unplaced version
        let port_ax2 iname cc (port:desmodule_port_t) =
            let shareable = false // for now
            (None, (shareable, instance.kind.form), iname, iname, port)::cc  
        let iname = instance.iname 
        let cc = List.fold (port_ax2 iname) cc instance.kind.ports
        cc

    let instance_ports =
        List.fold collect_instance_ports [] blockset

    // Find instance ports that need connecting
    let _ = vprintln 0 (msg + sprintf ": %i static ports" (length static_ports))
    let _ = vprintln 0 (msg + sprintf ": %i instance ports" (length instance_ports))    

    let (seeking_connection, other_ports) =
        // No ports on unused static resources need connection, although a tie off may be needed.
        // Master's on our own primary and subsidiary blocks need to be bound to a resouce and cannot be tied off.
        // 
        // On a used resource, an extra input to an overly-provisioned concentrator does not need a block - a tie off can be added.

        let needs_service_pred (zone_o, blocktap, iname, sname, port) = 
            //if port.masterf then blocktap = DF_primary ||blocktap = DF_subsidiary ||
            port.dp_needs_connection
        groom2 needs_service_pred (static_ports @ instance_ports) // Give priority to static ports in other_ports list. Bridges should not be included in this prelayout stage.

    let _ = vprintln 0 (msg + sprintf ": %i ports seeking connection" (length seeking_connection))    

    let portToStr (iname, (port:desmodule_port_t)) = sprintf "%s:%s/%s/%s" (if port.masterf then "Master" else "Slave") iname port.dp_name (vlnvToStr port.busbabs.vlnv)
    
    // Happily disconnected and unhappily disconnected:
    // Remnant server tracking is needed owing to lack or sharing for subsidiary blocks currently assumed.  We currently always assume static ports can be shared.
    let m_anciliary_notes = ref [] // TODO Put in settings and report on exit
    let anciliary_note ss = mutaddonce m_anciliary_notes ss // Expensive but useful ?
    let find_protocol_route port port_s =
        if port.busbabs.vlnv.kind = port_s.busbabs.vlnv.kind then
            (0, [])
        else             
            let _:(int * (int * int) list) [,] = interprotocol_matrix
            let (src, dest) = (protocol_idxof port.busbabs.vlnv.kind, protocol_idxof port_s.busbabs.vlnv.kind)
            if src<0 then
                let _ = anciliary_note (sprintf "No protocol adaptor converting from %s was found in library" (vlnvToStr port.busbabs.vlnv))
                (infinity, []) // Protocol endpoints are not one available in adaptor routes.
            elif dest<0 then
                let _ = anciliary_note (sprintf "No protocol adaptor converting to %s was found in library" (vlnvToStr port_s.busbabs.vlnv))
                (infinity, []) // Protocol endpoints are not one available in adaptor routes.
            else
                let (cost, route) = interprotocol_matrix.[src, dest]
                (cost, route)

    let env_collate _ = [] // For now

    let dropit wot lst =
        match wot with
            | None -> lst
            | Some(x0, y0, iname0, sname0, porta0)  ->
                let fpred (x, y, iname, sname, porta) = iname=iname0 && sname=sname0 && porta=porta0 // All tuple elements should match actually. Use deep equality.
                let (item, remainders) = list_remove fpred lst
                let _ = if nonep item then sf (sprintf "failed to remove port item %s %s %s" iname0 sname0 porta0.dp_name)
                remainders
    
    let (connections, fails, remnant_servers, domain_unifications, adaptors_rezzed) =
        let env = env_collate domain_unifications
        let rec wireup_iterate domain_unifications sofar fails other_ports adaptors_rezzed = function
            | [] -> (sofar, fails, other_ports, domain_unifications, adaptors_rezzed)
            | ((zone_o_client, (_, blocktap_client), iname_client, sname_client, port_client) as it_client) :: tt ->
                if blocktap_client=DF_zonebridge then wireup_iterate domain_unifications sofar fails other_ports adaptors_rezzed tt // Skip bridges at this stage
                else
                let rec find_server domain_unifications adaptors_rezzed = function
                    | [] -> (None, None, domain_unifications, adaptors_rezzed)
                    | ((zone_o, (shareable, blocktap_o), iname, sname, port) as worker)::others ->
                        let one_to_one = port.busbabs.onetoonef
                        let (okf, reason, route, domain_unifications, adaptors_rezzed) =
                            if blocktap_o = DF_zonebridge then (false, "skip-bridges", [], domain_unifications, adaptors_rezzed) // Ignore bridges at this stage
                            elif one_to_one then
                                if port.masterf=port_client.masterf then (false, "port-direction-mismatch", [], domain_unifications, adaptors_rezzed) // For one-to-one we need a master connected to a slave
                                else
                                    let (cost, protocol_route) = find_protocol_route port_client port
                                    if cost=infinity then (false, "no-protocol-route", [], domain_unifications, adaptors_rezzed)
                                    elif cost=0 then
                                        let (reason, severity, domain_unifications) = portmeta_mismatched msg domain_unifications port port_client // Direct connection
                                        (severity < 100, reason, [((iname, port), (iname_client, port_client))], domain_unifications, adaptors_rezzed)
                                    else
                                        let _ = vprintln 0 (sprintf "considera protocol-adapted route porta %s %s dom %s    for %s %s  dom %s" iname_client port_client.dp_name (domToStr env port_client.domain)    iname port.dp_name   (domToStr env port.domain) )                                        
                                        let protocol_route = map (fun (a,b)-> (rev_protocol_idxof a, rev_protocol_idxof b)) protocol_route 
                                        let _ = vprintln 3 (sprintf "Forming protocol-adapted route, %i protocol adaptors, using %A" (length protocol_route) protocol_route)
                                        let rez_adaptor (init_side, targ_side) =
                                            let (aa, master_port, slave_port) = find_adaptor_block_serf ww msg  init_side targ_side // initiating left to right - left port should be bindable to a master and so is itself a slave - TODO please freshen domain name
                                            let a_iname = funique "a_iname"
                                            (aa, a_iname, master_port, slave_port)
                                        let adaptor_instances = map rez_adaptor protocol_route  
                                        let adaptors_rezzed = (map f1o4 adaptor_instances) @ adaptors_rezzed
                                        let rec wireup_intermediate_assocs = function
                                            | xx::yy::tt -> ((f2o4 xx, f4o4 xx), (f2o4 yy, f3o4 yy)) :: wireup_intermediate_assocs (yy::tt)
                                            | _ -> []
                                        let associations =
                                            let (top, tail) = (hd adaptor_instances, hd (rev adaptor_instances))
                                            ((iname_client, port_client), (f2o4 top, f3o4 top)) :: wireup_intermediate_assocs adaptor_instances @ [ ((f2o4 tail, f4o4 tail), (iname, port)) ]
                                        let (okf, severity, domain_unifications) =
                                            let scox (okf, severity, domain_unifications) (((i_lhs, lhs), (i_rhs, rhs)), hop_no) =
                                                let _ = if lhs.masterf=rhs.masterf then vprintln 0 (sprintf "+++ invalid protocol route link: hop_no=%i/%i (master not connected to slave) between lhs=%s %A and rhs=%s %A" hop_no (length associations) i_lhs lhs i_rhs rhs)
                                                if false && lhs.masterf=rhs.masterf then sf (sprintf "invalid protocol route link: hop_no=%i/%i (master not connected to slave) between lhs=%s %A and rhs=%s %A" hop_no (length associations) i_lhs lhs i_rhs rhs)                                                
                                                else
                                                    let (reason_, severity1, domain_unifications) = portmeta_mismatched msg domain_unifications lhs rhs
                                                    let _ = vprintln 3 (sprintf "  protocol route hop report %i  %s" severity1 reason_)
                                                    (okf && severity1 < 100, max severity severity1, domain_unifications)
                                            List.fold scox (true, 0, domain_unifications) (zipWithIndex associations)
                                        // TODO make/record connections
                                        (okf, "aok-protocol-routed", associations, domain_unifications, adaptors_rezzed)
                            else muddy "multicast ok check"
                        let _ = vprintln 0 (sprintf "considera porta %s %s dom %s:     worker?= %s %s  dom %s   status=%s okf=%A" iname_client port_client.dp_name (domToStr env port_client.domain)    iname port.dp_name (domToStr env port.domain)  reason okf)
                        if okf then
                            let chid = [ funique "chassoc" ]
                            let chids = if length route > 1 then map (fun (_, n) -> i2s n :: chid) (zipWithIndex route) else [ chid ]
                            let gencon (hchid, ((iname_l, port_l), (iname_r, port_r))) =
                                let zone = None // This will be filled in on second pass when we know the zones after bridging
                                (hchid, consort [ (iname_l, "", zone, port_l); (iname_r, "", zone, port_r) ])
                            let new_connections = map gencon (List.zip chids route)
// TODO order of search needs randomising here.
// We can share at mid points of the protocol route with a minor improvement to this code. TODO.
                            let used_up = if shareable then None else Some worker
                            (Some new_connections, used_up, domain_unifications, adaptors_rezzed)
                        else
                            find_server domain_unifications adaptors_rezzed others
                let (newbind, used_up, domain_unifications, adaptors_rezzed) = find_server domain_unifications adaptors_rezzed tt // First look in list of other ports that want connection.
                let (newbind, other_ports, domain_unifications, adaptors_rezzed, tt) =
                    if not_nonep newbind then
                        let tt = dropit used_up tt
                        let _ = vprintln 3 (sprintf "x4 used tt entry")
                        (newbind, other_ports, domain_unifications, adaptors_rezzed, tt) 
                    else
                        let (newbind, used_up, domain_unifications, adaptors_rezzed) = find_server domain_unifications adaptors_rezzed other_ports // If nothing suitable, lookup in other resources.
                        if nonep newbind then
                            let _ = vprintln 3 (sprintf "x4 failed to use any entry")
                            (newbind, other_ports, domain_unifications, adaptors_rezzed, tt) 
                        else
                            let _ = vprintln 3 (sprintf "x4 used other_ports entry")
                            let other_ports = dropit used_up other_ports
                            (newbind, other_ports, domain_unifications, adaptors_rezzed, tt) 

                if not_nonep newbind then wireup_iterate domain_unifications (valOf newbind @ sofar) fails other_ports adaptors_rezzed tt
                else wireup_iterate domain_unifications sofar ((iname_client, port_client)::fails) other_ports adaptors_rezzed tt
                
        wireup_iterate domain_unifications [] [] other_ports [] seeking_connection
    let _ = vprintln 0 (msg + sprintf ": connected groups=%i, needing connection=%i:\nbeing=\n%s" (length connections) (length fails) (sfoldcr_lite (fun x -> "   " + portToStr x) fails))


    // If fails contains a matching master and slave of same domain then join ? TODO

    // Find preferably a static port or else a subs block that meets the open port needs of a member of the fails list.



    let new_subsidiary_blocks = 
        let subsidiary_blocks =
            let scan_subs cc (iname, port) =

                let has_such_a_slave_port block =
                    let rec searchin2 = function
                        | [] -> false
                        | port_s::tt when port_s.busbabs.vlnv.kind=port.busbabs.vlnv.kind && port_s.masterf=false -> true
                        // TODO: let (_, severity, domain_unifications) = portmeta_mismatched msg domain_unifications port port_s
                        | _ :: tt -> searchin2 tt
                    searchin2 block.ports 
                let rec searchin = function
                    | [] ->
                        cleanexit(msg+ sprintf ": Cannot find any registered %s IP block to instantiate so as to serve " form + portToStr (iname, port))
                    | (_, block)::tt when has_such_a_slave_port block ->
                        let newinstance = { nailed_=None; kind=block; iname=funique ("for" + iname) }
                        let _ = vprintln 3 (sprintf "Instantiating kind=%s iname=%s to serve port %s of %s kind=%s" block.name.kind newinstance.iname port.dp_name iname "")
                        newinstance :: cc
                    | _ :: tt -> searchin tt

                searchin !g_temporary_block_database 
            List.fold scan_subs [] fails
        let _ = vprintln 1 (sprintf " closer %s iteration %i: No of new subsidiary IP blocks is %i " form iteration (length subsidiary_blocks))
        map (fun x->(false,x)) subsidiary_blocks

    (connections, new_subsidiary_blocks, domain_unifications)


let system_integrator_constructive_placer ww settings attempt_no inventory_blocks =
    let msg = "system_integrator_constructive_placer"
    let ww = WF 3 msg ww (sprintf "Start attempt %i" attempt_no)

    let m_prbv = ref 1  // settings.prng
    let prbv n =
        mutinc m_prbv 1
        !m_prbv % n

    let zones = !g_soclayout_zone_list 
    let n_zones = length zones
    
    let rec build sofar (pf_, block) =
        let where =
            if not_nonep block.nailed_ then
                match g_soclayout_zone_database.lookup (valOf block.nailed_) with
                    | Some zone -> zone
                    | None -> sf ("missing nailed-to zone " + valOf block.nailed_)
            else select_nth (prbv(n_zones)) zones
        DES_augment(block.iname, { what=block; where=where }, sofar)


    List.fold build DES_empty inventory_blocks
 

// Write dot visualisation of partitioning of logic to zones and FPGAs
let system_integrator_writeout_dot_plot ww keyname dot0 =
    let ww = WF 1 "system_integrator_writeout_dot_plot" ww "start"
    let filename = "system_integrator_viz_" + keyname + ".dot"
    let fd = yout_open_out filename
    let _ = yout(fd, report_banner_toStr "// ")
    let _ = dotout fd (DOT_DIGRAPH("system_integrator_" + keyname, list_flatten dot0))
    let _ = yout_close fd
    let ww = WF 1 "system_integrator_writeout_dot_plot" ww "done"
    ()


// Warshall's algorithm is also applied to protocol adaptors in the library, to see what can be connected to what in principle and the best pattern of adaptors, giving each adaptor a unit cost at this time.
// We must avoid building wandering chains that convert backwards and forwards between protocols, but as Warshall considers each protocol a node in a multi-hop journey, it will only instantiated at most one of each type of adaptor in a path.
let gen_protocol_adapting_matrix ww settings msg  adaptor_database =
    let ww = WF 3 "gen_protocol_adapting_matrix" ww "start"
    let database: ListStore<string * string, desmodule_t> = adaptor_database
    let protocol_names =
        let m_r = ref []
        let azz (src, dest) = (mutaddonce m_r src; mutaddonce m_r dest)
        let _ = for k in adaptor_database do azz k.Key done
        !m_r
    let protocol_enumeration = zipWithIndex protocol_names
    let pairs0 = all_pairs protocol_names // We want the full cartesian product here whereas bridges are undirected and so a lower triangle is fine.x
    let protocol_idxof name = valOf_or (op_assoc name protocol_enumeration) -1
    let paths0 =
        let m_s = ref []
        let azy (src, dest) = mutadd m_s (protocol_idxof src, protocol_idxof dest, 1) // Here we assume unit cost for any adaptor, but delay, throughput and area are measured accurately in final cost metric.
        let _ = for k in adaptor_database do azy k.Key done
        !m_s
    //let _ = dev_println (sprintf "Initial interprotocol arcs %A" paths0)
    let (interprotocol_matrix, infinity) = floyd_warshall true (length protocol_names) paths0

    let rev_protocol_idxof =
        let pop = map (fun (a,b) -> (b,a)) protocol_enumeration
        let rev_protocol_idxof n =
                match op_assoc n pop with
                    | Some ans -> ans
                    | None -> sf (msg + sprintf ": Cannot reverselookup protocol no %i" n)
        rev_protocol_idxof

    let _ =
        let banner = "Protocol Adaption Cost Matrix"
        let _:(int * (int * int) list) [,] = interprotocol_matrix
        let table =          
            if length protocol_names < 2 then // Report interprotocol_matrix
                banner + sprintf ": there are only %i protocol_names, so no matrix needed." (length protocol_names)
            else
                let zone_names = map (fun x->x) protocol_names
                let rows =
                    let rowgen name =
                        let lookup dest =
                            let (cost, route) = interprotocol_matrix.[protocol_idxof name, protocol_idxof dest]
                            if cost = infinity then "." else i2s cost
                        name :: (map lookup zone_names)
                    map rowgen zone_names
                tableprinter.create_table(banner, "From/To" :: zone_names, rows)

        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        ()
 
    let ww = WF 3 "gen_protocol_adapting_matrix" ww "finished"           
    (protocol_names, interprotocol_matrix, infinity, protocol_idxof, rev_protocol_idxof)

// Transitive closure of inter-zone links with minimal costs:
let gen_zone_interconnection_matrix ww msg zones static_ports = 
    let ww = WF 3 "gen_zone_interconnection_matrix" ww "start"
    // We simplistically find the lowest cost path regardless of how it needs adapting and without load balancing for now.
    let find_bridges zfrom zto = // Need perhaps only to look in the static blocks
        let bfind zone masterf lst =
            let bfp = function
                | (Some zone_o, form_, iname, sname, port) ->
                    zone_o = zone && port.masterf = masterf
                | _ -> sf (msg + ": find_bridge:  static ports should have a zone " + zfrom)
            List.filter bfp lst
        let b1 = bfind zfrom false static_ports  // Slave on origin side
        let b2 = bfind zto true static_ports     // Master on far side
        let porto = list_intersection(map f3o5 b1, map f3o5 b2)
        porto

    let zone_names = map (fun x->x.name) zones
    let zoneidx = zipWithIndex zone_names
    let pairs0 = lower_product [] zones
    let idxof name = valOf_or_fail "idxof" (op_assoc name zoneidx)
    let paths0 = List.fold (fun cc (a, b) -> if not_nullp (find_bridges a.name b.name) then (idxof a.name, idxof b.name, 1)::cc else cc) [] pairs0
    //let _ = dev_println (sprintf "initial arcs %A" paths0)
    let (matrix, infinity) = floyd_warshall true (length zones) paths0

    let rev_idxof =
        let pop = map (fun (a,b) -> (b,a)) zoneidx
        let rev_idxof n =
            match op_assoc n pop with
                | Some ans -> ans
                | None -> sf (msg + sprintf ": Cannot reverselookup node %i" n)
        rev_idxof
    let ww = WF 3 "gen_zone_interconnection_matrix" ww "finished"            
    (matrix, infinity, idxof, rev_idxof)
        

let rec des_flatten cc = function
    | DES_empty -> cc
    | DES_augment(iname, placement, rest_of_design) -> des_flatten (placement::cc) rest_of_design

type blocks_by_zone_t = ListStore<string, block_placement_t>

let fullness_report ww msg settings banner zones design =
    let (blocks_collated_by_zone, blocks_to_zone) =     // Collate on layout zone - first time
        let blocks_collated_by_zone = new blocks_by_zone_t("blocks_by_zone")
        let blocks_to_zone = new OptionStore<string, string>("block_to_zone")        
        let addf block_placement =
            blocks_collated_by_zone.add block_placement.where.name block_placement
            blocks_to_zone.add block_placement.what.iname  block_placement.where.name 
        app addf (des_flatten [] design)
        (blocks_collated_by_zone, blocks_to_zone)

    let zfreport zone =
            let items: (block_placement_t list)  = blocks_collated_by_zone.lookup zone.name

            let banner = banner + ": Blocks in zone " + zone.name
            let zblk_report placement =
                let portdoms = sfold (fun port -> sprintf "%A" port.domain) placement.what.kind.ports                
                [ placement.what.kind.name.kind; placement.what.iname; portdoms; uom_report placement.what.kind.area ]

            let item_lines = map zblk_report items
            let (totals, will_not_fit, utils) =
                if nullp items then ("ZERO", false, "Empty zone") else
                    let sum = List.fold (fun cc x->uom_sum cc x.what.kind.area) (uom_zero (hd items).what.kind.area) items
                    let utils = sprintf "Percentage utilization in %s is %2.1f %%\n" zone.name (double(int(1000.0 * sum.value/zone.area.value))/10.0)
                    (uom_report sum, uom_dgtd sum zone.area, utils)
            let total_line = [ "TOTAL"; ""; ""; totals ]
            let table = tableprinter.create_table(banner, [ "Block Kind";  "Block Instance Name"; "Block port domains"; "Area" ], item_lines @ [total_line])
            let _ = mutadd settings.table_reports table
            let _ = aux_report_log msg [table; utils]
            //let _ = vprintln 1 table
            will_not_fit
    let ans = map zfreport zones
    (blocks_collated_by_zone, blocks_to_zone, ans)





// We need to collate the concentrators available for a given protocol by fanin and fan out
let soc_instantiator_get_concs ww msg settings  collated_concentrators_by_protocol (busbabs:ip_xact_vlnv_t)  =
    let protocol:string = busbabs.kind
    match settings.p_concs_sortedout.lookup protocol with
        | Some ans -> ans

        | None ->
           match op_assoc protocol collated_concentrators_by_protocol with
            | None -> cleanexit (msg + sprintf ": no concentrators found in library for protocol " + vlnvToStr busbabs)
            | Some items ->
                let by_arity desmod = length desmod.ports
                let compare_arity (a, b) (c, d) = a-c 
                //Sort into decreasing arity so we can easily select between using the largest suitable concentrator with tie offs of unused ports or else instantiate a combination of smaller concentrators.
                let items = List.sortWith compare_arity (generic_collate by_arity items)

                // For each arity we should find a mux/demux pair for this protocol.

                let items =
                    let divide (arity, concentrators) =
                        let (muxes, demuxes) = groom2 (fun desmod -> hd(desmod.ports).masterf) concentrators // A mux has a master focus port.
                        (arity-1, muxes, demuxes) // Remove one from arity owing to the focus port having been counted.
                    map divide items

                // Once collated in this way it is worth printing in a table and memoising for further lookups on this protocol
                let table =
                    let banner = "Concentrators available for protocol " + protocol
                    let namef (desmod:desmodule_t) = vlnvToStr desmod.name
                    let rowgen (arity, muxes, demuxes) = [ i2s arity; sfold namef muxes; sfold namef demuxes]
                    let rows = map rowgen items
                    let table = tableprinter.create_table(banner, [ "Arity"; "Tagging Mux"; "Demux" ], rows)
                    let _ = mutadd settings.table_reports table
                    let _ = aux_report_log msg [table]
                    ()
                    

                // sf (msg + sprintf ":found concentrators for protocol %s with following fanin/out: %s"  protocol (sfold (fun(a, _, _)->i2s(a-1)) items))
                let _ = settings.p_concs_sortedout.add protocol items
                items 



    
//
//   
//
let instantiate_some_concentrators ww msg settings collated_concentrators_by_protocol arb_sel connections = 
        // Instantiate concentrators - find multiply bound ports - these need concentrators if they only natively support one-to-one binding.

    let portassocs = new ListStore<string * string, string list>("portassocs")   // Indexed by pair (block iname * port instance name)
    let chidinfo = new OptionStore<string list, conquad_t list>("chidinfo") 

    let get_connection chid =
        match chidinfo.lookup chid with
            | Some peerset -> peerset
            | None -> sf ("missing chid (connection identifier) info for " + hptos chid)
            
    let collate_portbind (chid, peers) =
        let collate_portbind1 (of_what, bridge_iname, zone_o, porta) =
            let key = if bridge_iname = "" then of_what else bridge_iname
            let _ = dev_println (sprintf "biname=%s    key=%s " bridge_iname key)
            portassocs.add (key, porta.dp_name) chid
            chidinfo.add chid peers
        app collate_portbind1 peers

    let _ = app collate_portbind connections

    let list_form =
        let m_r = ref []
        for k in portassocs do mutadd m_r (k.Key, k.Value) done
        !m_r

    let get_concs (busbabs:ip_xact_vlnv_t)  = soc_instantiator_get_concs ww msg settings  collated_concentrators_by_protocol busbabs
       
    let concentrators_required =
        let conc_needed_pred ((of_what, dp_name), chids) = (length chids >= 2 (* && not one-to-one *))                     // TODO check port allowable binding arity
        List.filter conc_needed_pred list_form

    // This will be an even number of conentrators normally and so we apply tagging muxes and demuxes to each half of each pair.
    // But when applied to a static port only one direction is needed.
    let _ =
        let ax arg =
            let ((of_what, dp_name), chids) = arg
            sprintf "  of_what=%s dp_name=%s  ^=%i chids=%s" of_what dp_name (length chids)(sfold hptos chids)
        let needing_work = sfoldcr_lite ax concentrators_required
        vprintln 0 (msg+ sprintf ": Concentrators needed are for:\n" + needing_work)

    let collated_by_name =
        let pivotf = (fun ((of_what, _), _) -> of_what)
        generic_collate pivotf concentrators_required

    let get_concentrator_data exa =
        match op_assoc exa connections with
            | Some ((of_what, _, zone, porta)::_) -> // they should all have the same bus type, matching down to portmeta parameters. 
                (porta.busbabs, get_concs porta.busbabs.vlnv, zone)
            | _ -> sf "L1173"

    let make_conc_plan arity_needed concentrators_available = // Decide what degree of fan-in/out to deploy at this point.
        let rec makeplan = function
            | [] -> sf (msg + " Concentrator plan failed - was there no diadic concentrator in the library ?")
            | (a, muxes, demuxes)::tt -> if a <= arity_needed then (a, muxes, demuxes) else makeplan tt
        makeplan concentrators_available
        
    let rec zone_concensus wot sofar = function // All members should be in one zone - find out its name.
        | (_,       _, None, _) -> sofar
        | (of_what, _, Some x, _) ->
            if nonep sofar then Some x
            elif valOf sofar <> x then
                hpr_yikes (msg + sprintf ": zones mismatched %s cf %s for %s" (valOf sofar) x of_what)
                sofar
            else sofar

    let add_concentrator (cc, cd) = function
        | (name, [((l_of_what, l_dp_name), ll); ((r_of_what, r_dp_name), rr)]) -> // Symmetric concentration with an in-site and an out-site (typically on different sides of an inter-zone bridge).
            //let llc = List.filter (fun s->s.x = ll) connections 
            let (protocol, concentrators_available, zone_needed) = get_concentrator_data (hd ll)

            let rec pair_up no otherside = function
                | [] -> []
                | ll::tt ->
                    // TODO check all the same protocol and initiator direction - unifier will do that?
                    let predf x = hd (rev x) = hd (rev ll)
                    match list_remove predf otherside with
                        | (Some rr, otherside) ->
                            (no, ll, rr) :: pair_up (no+1) otherside tt
                        | (None, _) -> sf (msg + sprintf ": concentrator construction faced unmatched structure")

            let _ = vprintln 0 (sprintf "conc_needed: of_what=%A ll=%A rr=%A" name ll rr)
            // Concentrators applied to static ports do not need to be available in pairs, since the `other' side is not under our control.
            if length ll <> length rr then cleanexit(msg + sprintf ": Cannot add straightforward concentrator/deconcentrator for connection %s owing to different number of ports each side : %i cf %i" (name) (length ll) (length rr))
            let matchings = pair_up 0 rr ll 

            let arity_needed = length matchings
            let (arity_applied, mml, ddl) = make_conc_plan arity_needed concentrators_available // We can concentrate any amount on this iteration with remaining work being picked up on another iteration. Start with plain greedy.  Tieoffs are needed if there is no arity=2 concentrator.
                

            let mm:desmodule_t = arb_sel mml
            let dd             = arb_sel ddl
            let site = funique (protocol.vlnv.kind)

            let (mm_ports, mm_focus) = (tl mm.ports, hd mm.ports)
            let (dd_focus, dd_ports) = (hd dd.ports, tl dd.ports)

            let (mux, demux, connection_edits)  =
                // The list split 
                let (concentrated, remainers_) = list_split arity_applied matchings  // The fan-in/out may be greater than the available muxs in the library, so perhaps only do part of it on this iteration.
                let (p1, p2) = (map f2o3 concentrated, map f3o3 concentrated) // unzipped it! oops
                let (portas1, portas2) = (map get_connection p1, map get_connection p2)
                let (lc1, rc1) = (length (list_once(map hd portas1)), length (list_once(map cadr portas1)))  // Focus point for a mux is a common rhs, and for a demux is a common lhs.
                let (lc2, rc2) = (length (list_once(map hd portas2)), length (list_once(map cadr portas2)))  // Tally to see what we are doing.
                let _ = vprintln 3 (sprintf " fanin l tallies %i %i,  fanout r tallies %i %i" lc1 rc1   lc2 rc2)
                let (bnode_l, peers_l, bnode_r, peers_r) =
                    if lc1=1 && rc2=1 && rc1=lc2 then
                        (once "XX-L" (list_once(map cadr portas2)), List.zip p2 (map hd portas2), once "XX-R" (list_once((map hd portas1))), List.zip p1 (map cadr portas1))
                    elif rc1=1 && lc2=1 && lc1=rc2 then
                        (once "XY-L" (list_once(map cadr portas1)), List.zip p2 (map hd portas1), once "XY-R" (list_once((map hd portas2))), List.zip p1 (map cadr portas2))
                        
                    else sf (msg +  sprintf ": many-to-many concentrator instantiatiation (switch/NoC synthesis not yet supported concentrating between %s/%s and %s/%s" l_of_what l_dp_name r_of_what r_dp_name)

                let zone_l = valOf_or_fail "zone_l" (List.fold (zone_concensus "") None (map snd peers_l))
                let zone_r = valOf_or_fail "zone_r" (List.fold (zone_concensus "") None (map snd peers_r))

                let mux =   (zone_l, site + "_mux", mm) // where, iname and what
                let demux = (zone_r, site + "_demux", dd)

                let  _ = vprintln 0 (sprintf "arity_needed=%i arity_applied=%i Concentrated=%A" arity_needed arity_applied concentrated )
                let newlinks =
                    let al =
                            let gec (((chid, port), b), no) =
                                let b = (f2o3 mux, "", Some zone_l, b)  
                                ("MUXi" + i2s no :: chid, [port; b])
                            map gec (zipWithIndex(List.zip peers_l mm_ports))
                    let b = ("MUX" :: [site],    [(f2o3 mux,   "", Some zone_l, mm_focus); bnode_l ])
                    let c = ("DEMUX" :: [site],  [bnode_r; (f2o3 demux, "", Some zone_r, dd_focus) ])
                    let dl =
                            let gec ((a, (chid, port)), no) =
                                let a = (f2o3 demux, "", Some zone_r, a)
                                ("DEMUXi" + i2s no :: chid, [a; port])
                            map gec (zipWithIndex(List.zip dd_ports peers_r))
                    al @ [ b; c ] @ dl
                (mux, demux, (List.fold (fun cc (arity, a, b) ->a::b::cc) [] concentrated, newlinks))
            (mux :: demux :: cc, connection_edits :: cd)

        | (name, [((l_of_what, l_dp_name), ll)]) -> 
    
            let (protocol, concentrators_available, zone_needed) = get_concentrator_data (hd ll)
            let arity_needed = length ll
            // We will only need one of mml and ddl at this point, but we require the other to be in the library!
            let (arity_applied, mml, ddl) = make_conc_plan arity_needed concentrators_available // We can concentrate any amount on this iteration with remaining work being picked up on another iteration. Start
            let (concentrated_chids, remainers_) = list_split arity_applied ll  // The fan-in/out may be greater than the available muxs in the library, so perhaps only do part of it on this iteration.
            let concentrated = map (fun chid -> (chid, get_connection chid)) concentrated_chids
            let portas = map snd concentrated
            let (lc1, rc1) = (length (list_once(map hd portas)), length (list_once(map cadr portas)))  // Focus point for a mux is a common rhs, and for a demux is a common lhs.

            let _ = vprintln 3 (sprintf " fanin/out tallies lc=%i rc=%i" lc1 rc1)
            let (muxf, focus_node, peers) =
                if lc1>1 && rc1=1 then
                    (false, once "SMUX" (list_once(map cadr portas)), map (fun (chid, ports)->(chid, hd ports)) concentrated)
                elif lc1=1 && rc1>1 then
                    (true,  once "SDMUX" (list_once(map hd portas)), map (fun (chid, ports) ->(chid, cadr ports)) concentrated)
                else sf (msg +  sprintf ": many-to-many concentrator instantiatiation in muxing/demuxing of %s/%s  la=%i ra=%i" l_of_what l_dp_name lc1 rc1)

            let zone = valOf_or_fail "asym-conc-zone" (List.fold (zone_concensus "") None (map snd peers))

            let  _ = vprintln 0 (sprintf "arity_needed=%i arity_applied=%i Concentrated=%A" arity_needed arity_applied concentrated )
            //7~muddy(msg + sprintf ": Add asymmetric concentrator/deconcentrator (for static port typically) %s arity_needed=%i" (name) (length ll))
            let tag = if muxf then "_asym_mux" else "_asym_demux_"
            let md = arb_sel (if muxf then mml else ddl)
            let site = funique (protocol.vlnv.kind)
            let conc = (zone, site + tag, md)
            let (conc_ports, conc_focus) = (tl md.ports, hd md.ports)
            let connection_edits =
                let newlinks =
                    if muxf then
                        let al =
                            let gec (((chid, peer_porta), mux_porta), no) =
                                let b = (f2o3 conc, "", Some zone, mux_porta)  
                                ("MUXi" + i2s no :: chid, [peer_porta; b])
                            map gec (zipWithIndex(List.zip peers conc_ports))
                        let b = ("MUX" :: [site],    [(f2o3 conc,  "", Some zone, conc_focus); focus_node ])
                        al @ [ b ]
                    else
                        let c = ("DEMUX" :: [site],  [focus_node; (f2o3 conc, "", Some zone, conc_focus) ])
                        let dl =
                            let gec ((demux_porta, (chid, peer_porta)), no) =
                                let a = (f2o3 conc, "", Some zone, demux_porta)
                                ("DEMUXi" + i2s no :: chid, [a; peer_porta])
                            map gec (zipWithIndex(List.zip conc_ports peers))
                        c :: dl
                (concentrated_chids, newlinks)

            (conc :: cc, connection_edits :: cd)

        | (name, llst) ->
            cleanexit(msg + sprintf ": Cannot add concentrator/deconcentrator for connection %s owing to having more than two halves: %i " (name) (length llst))

    let (concentrators_instantiated, connection_edits) = // TODO use balanced tree instead of skew tree here
        List.fold add_concentrator ([], []) collated_by_name

    let (old_for_delete, new_to_keep) = List.unzip connection_edits
    let connections =
        let deleted = List.fold (fun (m:Map<string list, int>) s -> m.Add(s, 1)) Map.empty (list_flatten old_for_delete)
        // Now apply edits to the connections
        let undeleted_pred (chid, _) = nonep(deleted.TryFind chid)
        List.filter undeleted_pred connections
    (concentrators_instantiated, list_flatten new_to_keep @ connections)


// Perform mini-audit on how many layout zones are mentioned inside a single connection.
let arcs_compatible_zones_audit peers =
    let zonef (of_what, _, zoneo, _) = zoneo
    let (unplaced, zones) = groom2 nonep (list_once(map zonef peers))
    (length unplaced, length zones)

let rec design_analyse ww design =
    let rec des_anal (count, zones) = function
            | DES_empty -> (count, zones)
            | DES_augment(iname, placement, rest_of_design) ->
                des_anal (count+1, singly_add placement.where zones) rest_of_design

    let (count, zones) = des_anal (0, []) design

    let cost = 1.0 // for now
    let _ = vprintln 3 (sprintf "Design attempt: count=%i zones=%s cost=%f" count (sfold (fun x->x.name) zones) cost)
    (count, zones, cost)



let construct_a_solution ww cX construction_no primary_blocks =       
    let (settings, collated_concentrators_by_protocol, zonewiring, adaptor_info) = cX
    let msg = settings.banner   
    let ww = WF 3  msg ww (sprintf "Start construction attempt %i" construction_no)
    let (static_ports, infinity, zone_interconnection_matrix, idxof, rev_idxof) = zonewiring
    let domain_unifications = []
    let (connections, inventory_blocks) =
        let rec iterate iteration blockset = 
             let (connections, new_ones, domain_unifications) = system_integrator_block_port_closer ww cX static_ports iteration domain_unifications blockset
             if nullp new_ones then (connections, blockset)
             elif iteration > settings.dlimit then sf (sprintf "Exceeded depth limit %i for transitive primary block instantiation." settings.dlimit)
             else iterate (iteration+1) (new_ones @ blockset)
        iterate 0 (map (fun x->(true,x)) primary_blocks)
        
    let _ = // This includes the directing shim
        let banner = "Primary and Subsidiary Blocks Needed"
        let xblk_report (pf, block) =
            let portdoms = sfold (fun port -> sprintf "%A" port.domain) block.kind.ports
            [ (if pf then "Primary" else "Subs"); block.kind.name.kind; block.iname; valOf_or_ns block.nailed_; portdoms; uom_report block.kind.area ]
        let table = tableprinter.create_table(banner, [ "Primary"; "Block Kind";  "Block Instance Name"; "Nailed"; "Block port domains"; "Area" ], map xblk_report inventory_blocks)
        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        //let _ = vprintln 3 table
        ()

    let unification_report ww banner domain_unifications =
        let ugroup_report lst =
            let is_const = function
                | DP_named_group _ -> true
                | _                -> false
            let (consts, vars) = groom2 is_const domain_unifications
            [ sfold (domToStr []) consts; sfold (domToStr []) vars ]
        let table = tableprinter.create_table(banner, [ "Constant"; "Variables" ], map ugroup_report domain_unifications)
        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        ()

       
    let connection_report ww banner connections =
        let xcon_report (chid, participants) =
            let (masters, slaves) = groom2 (fun (_, _, _, x)->x.masterf) participants
            let bfun (of_what, _, zone, port) = valOf_or_ns zone + " " + of_what + "  " + port.dp_name + " " + port.busbabs.vlnv.kind // + (if port.masterf then " M" else " S")
            let one_to_one = true // For now
            let linktype = if one_to_one then "1-to-1" else "Broadcast"
            let ms = sfold bfun masters
            let ss = sfold bfun slaves
            let interzone =
                match (masters, slaves) with
                    | (m::_, _) when nonep (f3o4 m) -> "(pre-layout)"
                    | (_::_::_, [s]) -> "Multimastered!"
                    | ([m], _::_::_) when one_to_one -> "IllegalFanOut!"
                    | ([m], [s]) ->
                        let zone_msg =
                            if nonep (f3o4 m) || nonep (f3o4 s) then "Unplaced"
                            elif f3o4 m <> f3o4 s then "Interzone" else "ok"                    
                        let (adaptor_msg, severity, domain_unifications__) = portmeta_mismatched  msg domain_unifications (f4o4 m) (f4o4 s)
                        zone_msg + adaptor_msg
                    | _ -> "malformed?"
            let kind = if nullp masters then "---??" else vlnvToStr (f4o4(hd masters)).busbabs.vlnv
            [ hptos chid; linktype; kind; ms; ss; interzone ]
        let table = tableprinter.create_table(banner, [ "Name"; "Link Type"; "Channel Kind";  "Initiator"; "Target(s)";  "Compatibility" ], map xcon_report connections)
        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        ()

    let _ =
        let banner = "Rat's Nest Connections"
        connection_report ww banner connections     // Report connections - rat's nest form.
        

    let _ = // sanity check

        let block_sanity_check (pf_, block) =
            let bsf_failed msg =
                cleanexit("soc_instantitor: sanity check failed: " + msg)
            if not_nonep block.nailed_ then
                match g_soclayout_zone_database.lookup (valOf block.nailed_) with
                    | Some _ -> ()
                    | None ->
                        bsf_failed ("Nailed to a non-existent zone" + valOf block.nailed_)

        app block_sanity_check inventory_blocks


    let ww = WF 1 msg ww (sprintf "All Primary and Subsidiary IP blocks added to the inventory. ")

    let arb_sel lst = hd lst // randomly select one using seeded prng.
    
    let design =
        let attempt_no = 0
        let raw_design = system_integrator_constructive_placer ww settings attempt_no inventory_blocks 
        raw_design

    let (count, zones, cost) = design_analyse ww design

    let _ = if count <= 0 then hpr_yikes(msg + ": No IP blocks in use !")
    let ww = WF 1 msg ww (sprintf "All IP blocks placed, we think. Total blocks=%i using %i zones (FPGAs)." count (length zones))


    let (blocks_collated_by_zone, blocks_to_zone, will_not_fit_) = // fullness report
        fullness_report ww msg settings "Initial Floorplan" zones design
        // TODO go round again if busted

    let _ =
        let banner = "Layout Zone Interconnection Costs"
        let _:(int * (int * int) list) [,] = zone_interconnection_matrix
        let table =          
            if length zones < 2 then // Report matrix
                banner + sprintf ": there are only %i layout zones, so no bridges needed." (length zones)
            else
                let zone_names = map (fun x->x.name) zones
                let rows =
                    let rowgen name =
                        let lookup dest =
                            let (cost, route) = zone_interconnection_matrix.[idxof name, idxof dest]
                            if cost = infinity then "." else i2s cost
                        name :: (map lookup zone_names)
                    map rowgen zone_names
                tableprinter.create_table(banner, "From/To" :: zone_names, rows)

        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        ()

    let ww = WF 1 msg ww (sprintf "All IP blocks placed, we think. Total blocks=%i using %i zones (FPGAs)." count (length zones))

    let connections = // hydrate connections with zone names
        let hydrate (chid, peers) =
            // Fields explicitly in the peers conquad_t are: component_instance, _, zone (When placed), porta)
            // The busbabs and dp_name fields of porta contain most of the details, but note that the component instance has an instance name and porta has a port instance name and these ultimately need logical catenation.
            let hydrate1 (of_what, foo_, zone_o, porta) =

                let zone =
                    match blocks_to_zone.lookup of_what with
                        | Some zone ->
                            if not_nonep zone_o then vprintln 1 (msg + sprintf "+++hmm, had old zone...")
                            Some zone
                        | None ->
                            match (porta:desmodule_port_t).dp_nailed with
                                | Some zone -> Some zone
                                | None ->
                                    let _ = vprintln 0 (msg + sprintf ": +++ Oh dear, no zone index for block %s and port %s" of_what porta.dp_name)
                                    None
                (of_what, foo_, zone, porta)

            (chid, map hydrate1 peers)
        map hydrate connections


    let _ =
        let banner = "Preliminary Routed Connections"
        connection_report ww banner connections     // Report connections - once preliminary layout complete


    // Constructively deploy bridges
    let find_paths zone_m zone_s =
        let (a, b) = (idxof zone_m, idxof zone_s)
        match zone_interconnection_matrix.[a, b] with
            | (cost, route) when cost=infinity ->
                cleanexit(msg + sprintf ": Unable to find any bridged route between zone %s and %s (in that direction)" zone_m zone_s)
            | (cost, route) ->
                let _ = vprintln 3 (sprintf "Between %s (%i) and %s (%i) we shall use route %A" zone_m a zone_s b route)
                (route)

#if OLD
    // Bridges could be defined to always have same protocol on both sides.
    // Bridges are duplex in data direction sense, but simplex in terms of initiating/targetting.
    let find_bridges zfrom zto = // Need perhaps only to look in the static blocks
        let bfind zone masterf lst =
            let bfp = function
                | (Some zone_o, iname, port) ->
                    zone_o = zone && port.masterf = masterf
                | _ -> sf (msg + ": find_bridge:  static ports should have a zone " + zfrom)
            List.filter bfp lst
        let b1 = bfind zfrom false static_ports  // Slave on source side
        let b2 = bfind zto true static_ports     // Master on far side
        let bridge_inames = list_intersection(map f2o3 b1, map f2o3 b2) // Find common inames
        let rec get_bridge iname = function
            | [] -> sf (msg + sprintf ": no such bridge %s" iname)
            | h::_ when f2o3 h = iname -> h
            | _::tt -> get_bridge iname tt

        let find_named_bridge iname =
            let bridge_has_name (zones, name, ramps) = (name = iname) //  && bridge.kind.form=DF_zonebridge)
            List.filter (bridge_has_name) !g_temporary_bridge_instance_database
            //(List.fold (fun cc zone->zone.static_contents@cc) [] blade_manifest)
        let suitable_bridges iname =
            match find_named_bridge iname with
                | [(zones, inama, (launch, landing)) as bridge] ->
                    let (to_ports, from_ports) = groom2 (fun d -> d.masterf) bridge.kind.ports
                    (bridge, from_ports, to_ports)
                    // old map (fun iname-> get_bridge iname static_ports) bridge_inames


                | [] -> cleanexit(msg + sprintf ": No bridge components are named '%s'"  iname)
                | items -> arb_sel items// prng
                
        map suitable_bridges bridge_inames
#endif


    // Bridges could be defined to always have same protocol on both sides.
    // Bridges are duplex in data direction sense, but simplex in terms of initiating/targetting.
    let find_bridges zfrom zto = // Need perhaps only to look in the static blocks
        let ok_pred ((f, t), _, _) = f=zfrom && t=zto
        let suitable_bridges =
            match List.filter ok_pred !g_temporary_bridge_instance_database with
                | [item] -> item
                | [] -> cleanexit(msg + sprintf ": No bridge components in manifest from '%s' to '%s'"  zfrom zto)
                | items -> arb_sel items// prng
        suitable_bridges


    let (connections, adaptor_blocks) =
        let bridge_zone_discrepancies (chid, peers) (cc, cd) =
            let (masters, slaves) = groom2 (fun (_, _, _, x)->x.masterf) peers
            match (masters, slaves) with
                | ([(iname_m, _, Some zone_m, port_m)], [(iname_s, _, Some zone_s, port_s)]) ->
                    if zone_m = zone_s then
                        let _ = vprintln 2 (msg + sprintf ": No bridge needed for %s" (hptos chid))
                        ((chid, peers)::cc, cd)
                    else // Either a bridge with the correct busabs is found or else we need to deploy adaptors and concentrators
                        let chosen_route =                            
                            match find_paths zone_m zone_s with
                                | [] ->
                                     cleanexit(msg + sprintf ": Unable to find a bridge between zone %s and %s (in that direction)" zone_m zone_s)
                                | route -> route // There is only one route kept - by warshall at the moment
                                // | [route] -> muddy (sprintf "bozz %A" route)
                                // | routes ->
                                //     let _ = vprintln 1 ("+++ need to load balance on routes TODO")
                                //    hd routes
                        let chosen_route = map (fun (a,b) -> (rev_idxof a, rev_idxof b)) chosen_route
                        let _ = vprintln 3 (msg + sprintf ": Using the following route for %s " (hptos chid) + sfold (fun (a,b)-> a + "->" + b) chosen_route)
                        //For now we'll just put adaptors on the edges and assume no intermediate adaptor at bridge sites is needed: i.e the bridges in and out of each stepping stone share a common protocol  - TODO
                        let make_a_hop ((zfrom, zto), hop_no) (cc, cd) =

                            let (bridge, launch_ramp, landing_ramp, from_port, to_port) = // Here we assume the bridge has same port type on both sides but we should support general case of course?
                                let ax bridge =
                                    match bridge with
                                    | (zones, iname, (launch, landing)) -> (bridge, launch, landing, arb_sel launch.kind.ports, arb_sel landing.kind.ports)  // If all bridges only have one suitable port then this is sufficient, else select or load balance over ports,
                                    //| (bridge, _, _) -> sf (msg + sprintf ": bridge does not have both a master and slave port. iname=%s kind=%s" bridge.iname (vlnvToStr bridge.kind.name))
                                match find_bridges zfrom zto with
                                    //| [] -> sf (msg + sprintf ":No bridge from %s to %s afterall!  Route=%A" zfrom zto chosen_route)
                                    | bridge -> ax bridge
                                    //| bridges ->
                                        //let _ = dev_println (msg + sprintf "Need to load balance bridges: TODO")//Or find one of the correct busbabs so no adaptor needed. Also should load balance.
                                        //ax(hd bridges)

                            let bridge_iname = f2o3 bridge
                            let (links_near, ads_l) =
                                let chid_hop_l = [ i2s hop_no; "LN"; ] @ chid  // Make some fresh channel segment names
                                let (_, severity, domain_unifications) = portmeta_mismatched msg domain_unifications port_m from_port
                                if severity < 100 then //if port_m.busbabs.kind = from_port.busbabs.kind then
                                    ([(chid_hop_l, [ (iname_m, "", Some zone_m, port_m); (launch_ramp.iname, bridge_iname, Some zone_m, from_port) ])], []) // Connect directly to bridge without adaptor
                                else
                                    let (adaptor, port_as, port_am) =
                                        let msg = msg + sprintf ": to cross near side of bridge %s" launch_ramp.iname
                                        find_adaptor_block ww msg port_m from_port   // initiating left to right
                                    let iname_a = sprintf "%s_adt_near_%i" (hptos chid) hop_no // Make adaptor instance name
                                    let chid_x = "NX" :: chid_hop_l
                                    let chid_y = "NY" :: chid_hop_l  
                                    let arcs =
                                        [
                                            // The port name and abstraction is in the final field
                                            (chid_x, [(iname_m, "", Some zone_m, port_m); (iname_a, "", Some zone_m, port_as)])
                                            (chid_y, [(iname_a, "", Some zone_m, port_am); (launch_ramp.iname, bridge_iname, Some zone_m, from_port)])
                                        ]
                                    (arcs, [(zone_m, iname_a, adaptor)])
                                
                            let (links_far, ads_r) =
                                let chid_hop_r = [ i2s hop_no; "LF"; ] @ chid
                                let (_, severity, domain_unifications) = portmeta_mismatched msg domain_unifications to_port port_s
                                if severity < 100 then // to_port.busbabs.kind = port_s.busbabs.kind 
                                    ([(chid_hop_r, [ (landing_ramp.iname, bridge_iname, Some zone_s, to_port); (iname_s, "", Some zone_s, port_s) ])], []) // Connect directly to far side of bridge   
                                else
                                    let (adaptor, port_as, port_am) =
                                        let msg = msg + sprintf ": to cross far side of bridge %s" landing_ramp.iname
                                        find_adaptor_block ww msg to_port port_s    // initiator first 
                                    let iname_a = sprintf "%s_adt_far_%i" (hptos chid) hop_no // Make adaptor instance name
                                    let chid_x = "FX" :: chid_hop_r
                                    let chid_y = "FY" :: chid_hop_r  

                                    let arcs =
                                        [
                                            (chid_x, [(landing_ramp.iname, bridge_iname, Some zone_s, to_port); (iname_a, "", Some zone_s, port_as)])
                                            (chid_y, [(iname_a, "", Some zone_s, port_am); (iname_s, "", Some zone_s, port_s)])
                                        ]
                                    (arcs, [(zone_s, iname_a, adaptor)])


                            (links_near @ links_far @ cc, ads_l @ ads_r @ cd)
                            
                        let (new_steps, new_adaptors) = List.foldBack make_a_hop (zipWithIndex chosen_route) ([], [])
                        (new_steps@cc, new_adaptors@cd)
                    
                | _ -> ((chid, peers)::cc, cd) // silently ignore other forms for now here - eg literal tieoffs that are broadcast
        List.foldBack bridge_zone_discrepancies connections ([], [])

    let _ =
        let banner = "Bridged and Adapted Connections"
        connection_report ww banner connections // After adaptors are inserted

        unification_report ww banner domain_unifications

    let _ = vprintln 2 (msg + sprintf ": %i adaptors needed to cross bridges" (length adaptor_blocks))


    let (concentrator_blocks, connections) =
        let rec iterate no blocks connections =
            let (concentrators_instantiated, connections) = instantiate_some_concentrators ww msg settings collated_concentrators_by_protocol arb_sel connections
            let _ = vprintln 3 (sprintf "Concentrator-add iteration %i added %i concentrators" no (length concentrators_instantiated))
            if nullp concentrators_instantiated then
                (blocks, connections)
            else iterate (no+1) (concentrators_instantiated@blocks) connections
        iterate 0 [] connections
        
        // Allocate tags
    let _ = vprintln 2 (msg + sprintf ": %i concentrator blocks needed to share resources" (length concentrator_blocks))
        // Link editx

    let newblocks = adaptor_blocks @ concentrator_blocks

    let _ =
        let banner = "Concentrated and Bridged Connections"
        connection_report ww banner connections     // Report connections - once preliminary layout complete

    let design = // Now add the bridge adaptors and concentrators to the design
        let add_new_block design (where, iname, newmodule) =
            let where =
                match g_soclayout_zone_database.lookup where with
                    | Some zone -> zone
                    | None      -> sf (msg + ": Placed block in a non-existent zone" + where)
            let newblock = { kind=newmodule; nailed_=None; iname=iname; }
            let placement = { what=newblock; where=where }
            DES_augment(iname, placement, design)
        List.fold add_new_block design newblocks

    let (count, zones, cost) = design_analyse ww design

    let (blocks_collated_by_zone, blocks_to_zone, will_not_fit_) = // fullness report
        fullness_report ww msg settings "End of Trial Floorplan" zones design
        // TODO go round again if busted
    (design, connections, cost)



let system_integrator_draw_diagram_using_dot ww (settings:soc_inst_settings_t) msg (zones, blocks_collated_by_zone, static_ports, connections) keyname =
    let ww = WF 1  msg ww ("start render graphic to " + keyname)
    let _:blocks_by_zone_t = blocks_collated_by_zone
    let _ = // Do a graphic
        let dotrender_zone zone =
            let items: (block_placement_t list)  = blocks_collated_by_zone.lookup zone.name

            let item_render item =
                let (style_, color) =
                    match item.what.kind.form with
                        | DF_primary      -> ("", "black")
                        | DF_subsidiary   -> ("", "black")
                        | DF_concentrator -> ("", "green")
                        | DF_zonebridge   -> ("", "orange")
                        | DF_adaptor      -> ("", "brown")
                        | _ -> ("", "red")
                let iname = item.what.iname

                // If a bridge or adaptor, want one port on each side. Directionally sorted.
                // If a concentrator or aggregator, want the focus port on one side and the remainder on the other side.
                let d_ports = 
                    let ports = item.what.kind.ports
                    let pf = (fun port -> sprintf "<%s> %s" (dot_nodesan port.dp_name) port.dp_name)
                    if length ports > 1 then
                       let focus = hd ports
                       let others = tl ports
                       let r = pf focus
                       let l = sfold_delim "|" pf others
                       let (l, r) = if focus.masterf then (l,r) else (r,l)
                       sprintf " {%s} | {%s} " l r
                    else
                        sfold_delim "|" pf ports
                let label = iname + "\n" + item.what.kind.name.kind + "| {" + d_ports + "}"
                let label = "{" + label + "}" // Lead off vertically
                [
                    DNODE_DEF(iname, [ ("label", label); ("shape", "record"); ("color", color)])

                    //DARC(DNODE(zone.name,""), DNODE(iname,""), [ ("color", "black"); ("label", "") ]) // Attach component to chip
                ]// @ list_flatten(map render_port item.what.kind.ports)

            let render_zone_connection cc (chid, peers) =
                let rec paintcon cc = function
                    | a::b::tt ->
                        let tailer = paintcon cc (b::tt)
                        let (n_unplaced, n_zones) = arcs_compatible_zones_audit [a;b]
                        let nf (of_what, _, zoneo, porta) = DNODE(of_what,  porta.dp_name)
                        if n_unplaced > 0 then
                            let _ = hpr_yikes ("no zone field in " + hptos chid)
                            tailer
                        elif n_zones < 2 then
                            let n = DARC(nf a, nf b, [ ("color", "black"); ("label", hptos chid) ]) // One link in a connection
                            n :: tailer
                        else
                            let _ = hpr_yikes ("dummy interzone node needed to render malformed " + hptos chid)                            
                            let dummy = funique (hptos chid + "dummy")
                            let n1 = DARC(nf a, DNODE(dummy + "near", ""), [ ("color", "yellow"); ("label", hptos chid) ])
                            let n2 = DARC(DNODE(dummy + "far", ""), nf b, [ ("color", "yellow"); ("label", hptos chid) ]) 
                            n1 :: n2 :: tailer
                    | _ -> cc
                let thiszone_pred = function
                    | None -> true
                    | Some z -> z=zone.name
                if disjunctionate (fun (of_what, _, zoneo, porta) -> thiszone_pred zoneo) peers then  paintcon cc peers else cc


            let x: (string option * (bool * desmodule_purpose_t) * string * string * desmodule_port_t) list  = static_ports
            let d_static_ports =
                let dotplot_static_port cc = function
                    | (Some z, (shareable, purpose), iname, sname, porta) when z=zone.name ->
                        let tag = if purpose = DF_zonebridge then "STATIC BRIDGE" else "STATIC PORT"
                        let label = sprintf "{%s | { <%s> %s } | %s }" iname (dot_nodesan porta.dp_name) porta.dp_name tag
                        let n = DNODE_DEF(sname, [ ("label", label); ("shape", "record"); ("color", "blue")])
                        n::cc
                    | _ -> cc
                List.fold dotplot_static_port [] static_ports

            let chip_subgraph = 
                [
                    //DNODE_DEF(zone.name, [ ("label", "Layout zone " + zone.name); ("shape", "square"); ("color", "brown")])

                ] @ list_flatten(map item_render items) @ (List.fold render_zone_connection [] connections) @ d_static_ports
            let ats = [("label", "Layout zone " + zone.name)] 
            [DSUB(["subgraph"; "cluster_" + zone.name  ], ats, chip_subgraph)]
        system_integrator_writeout_dot_plot ww keyname (map dotrender_zone zones)
    let ww = WF 1  msg ww ("finish render graphic to " + keyname)
    ()
    
// HPR System Integrator Main Entry Point
let system_integrator ww (settings:soc_inst_settings_t) primary_block_names =
    let msg = settings.banner
    let primary_block =
        let v = temp_block_assoc ww msg primary_block_names 
        {
            nailed_=  None
            iname=   "*top-primary-IP-block*"
            kind=    v
        }

    let primary_blocks: desmodule_instance_t list = [ primary_block ]

    let ww = WF 1  msg ww "start"
    let blade_manifest = [ example_chip1; example_chip2  ]
    let _ = app (install_layout_resource ww) blade_manifest
    let _ =
        let banner = "Blade Manifest"
        let bmp_report (chip:soclayout_zone_t) = [ chip.name; uom_report chip.area ]
        let table = tableprinter.create_table(banner, [ "Chip Name"; "Area" ], map bmp_report blade_manifest)
        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        //vprintln 3 table
        ()
    let ww = WF 1 msg ww "Blade Manifest Read In"

    // Find transitive closure of subsidiary IP blocks needed.
    let _ = vprintln 1 (sprintf " No of primary IP blocks is %i " (length primary_blocks))


    let static_ports:(string option(*zone*) * (bool * desmodule_purpose_t) * string(*iname*) * string (*side name*) * desmodule_port_t) list =  // Flattened - some inames are therefore repeated for multiport devices, with bridges spanning zones.
        let scan_static_ports cc zone =
            let spg cc instance =
                let shareable = true //default for static ports for now
                let port_ax iname cc (port:desmodule_port_t) = (Some zone.name, (shareable, instance.kind.form), iname, iname, port)::cc  
                let cc = List.fold (port_ax (instance.iname)) cc instance.kind.ports
                cc
            //let 
            List.fold spg cc zone.static_contents
        let b0 = List.fold scan_static_ports [] blade_manifest  // We are not interested in static ports of chips not in use

        let mine_bridge_ports cc ((from_zone, to_zone), iname, (launch_ramp, landing_ramp)) =
            let cc = List.fold (fun cc porta -> (Some from_zone, (false(*does not matter*), launch_ramp.kind.form(*Is DF_zonebridge*)),  iname, launch_ramp.iname,  porta)::cc)  cc launch_ramp.kind.ports
            let cc = List.fold (fun cc porta -> (Some to_zone,   (false, landing_ramp.kind.form), iname, landing_ramp.iname, porta)::cc)  cc landing_ramp.kind.ports
            cc
        let b1 = List.fold mine_bridge_ports b0 !g_temporary_bridge_instance_database
        b1

    let _ = // report static ports
        let env = []
        let banner = "Static (aka nailed) Ports in Blade Manifest"
        let masterf f = if f then "Initiator" else "Target"
        let sport_report (zone_o, form, iname, sname, (port:desmodule_port_t)) =
            // Todo report one-to-one here/
            [ valOf_or_ns zone_o; iname; sname; port.busbabs.vlnv.kind; (domToStr env port.domain); port.dp_name; sprintf "%A %s %s" (fst form) ((snd>>purposeToStr) form) (masterf port.masterf); "Zone/FPGA" ]
        let table = tableprinter.create_table(banner, [ "Zone/Chip"; "name2"; "Kind";  "Name"; "Domain"; "Side"; "Form"; "Zone/FPGA" ], map sport_report static_ports)
        let _ = mutadd settings.table_reports table
        let _ = aux_report_log msg [table]
        //vprintln 3 table
        ()


    let collated_concentrators_by_protocol =
        let gcp desmod =
            match desmod.ports with
                | example::_ -> example.busbabs.vlnv.kind
                | _ -> sf "L1145"
        generic_collate gcp !g_temporary_concentrator_instance_database

    let adaptor_info = gen_protocol_adapting_matrix ww settings msg g_adaptor_database
    let (protocol_names, interprotocol_matrix, infinity, protocol_idxof, rev_protocol_idxof) = adaptor_info

    let (zone_interconnection_matrix, infinity, idxof, rev_idxof) =
        let all_zones = !g_soclayout_zone_list 
        gen_zone_interconnection_matrix ww msg all_zones static_ports 

    let zonewiring = (static_ports, infinity, zone_interconnection_matrix, idxof, rev_idxof)
    let cX = (settings, collated_concentrators_by_protocol, zonewiring, adaptor_info) 
    let ww = WF 1  msg ww "Zone Interconnection Matrix Determined"

    // Make a number of design attempts and take best.
    let rec maketrials best no =
        let ans = construct_a_solution ww cX no primary_blocks 
        let (design, connections, cost) = ans
        let best = if nonep best || cost < f3o3(valOf best) then Some ans else best
        if no > 50 then best else maketrials best (no+1)

    let ans =
        match maketrials None 0 with
            | Some ans -> ans
            | None -> cleanexit(msg + ": No feasible design found.")
    let (design, connections, cost) = ans
            
    let (count, zones, cost) = design_analyse ww design
    let (blocks_collated_by_zone, blocks_to_zone, will_not_fit_) = // fullness report
        fullness_report ww msg settings "Final Floorplan" zones design

    let keyname = "roger" // for now
    let master_parameters = []
    let rtl_suffix = ".v"  // Verilog RTL
    let ip_integrator_vd = 3

    let _ = // Do a graphic
        if settings.render_dot then system_integrator_draw_diagram_using_dot ww settings msg (zones, blocks_collated_by_zone, static_ports, connections) keyname


    // For each zone we write an IP-XACT design and an RTL file.
    let write_zone ww zone =
        
        let ww = WF 1 msg ww (sprintf "Start write output files for zone %s" zone.name)
        let output_file_root = keyname + zone.name
        let msg = msg + ": zone " + output_file_root
        let zone_filename_rtl = output_file_root + rtl_suffix
        let zone_filename_ip_xact = output_file_root // ".xml" suffix is added by output routine. 

        let items: (block_placement_t list)  = blocks_collated_by_zone.lookup zone.name

        let op_vlnv = { vendor="HPRLS"; library="user"; kind=zone.name; version="1.0" }
        let hl_component_instances =
            let maz_instance block =
                let desi = block.what
                {
                    vlnv=                desi.kind.name // 
                    generated_by=        "HPR_IP_INTEGRATOR"
                    
                    definitionf=         false    // When definitionf holds, not an instance.
                    iname=               desi.iname
                    
                    externally_provided= true
                    external_instance=   true
                    preserve_instance=   true
                    in_same_file=        false
                }
            map maz_instance items

        let hl_connections = // Convert connections into high-level form for IP-XACT design document.
            let gec_tmep cc (chid, peers) =
                if length peers <> 2 then
                    let _ = hpr_yikes ("// TODO broadcast ignored ..." + hptos chid)
                    cc
                else
                    let (n_unplaced, n_zones) = arcs_compatible_zones_audit peers
                    if n_zones < 2 then
                       let boz (of_what, _, zone_o, porta)  =
                         { protocols.g_null_tmep with
                              busref= porta.dp_name
                              iiname= of_what
                         }
                       let aa = boz (hd peers)
                       let bb = boz (cadr peers)
                       (hptos chid, aa, bb)::cc
                    else cc
                    
            List.fold gec_tmep [] connections
            

        let _ = vprintln 2 (msg + sprintf ": %i/%i high-level bus connections between %i components"  (length hl_connections) (length connections) (length items))

        // Write as IP-XACT Design Document
        let _ =
            if settings.render_ip_xact then
                let ww = WF 1 msg ww (sprintf "Start write IP-XACT design file %s for zone %s" zone_filename_ip_xact zone.name)
                let files = [ zone_filename_rtl ]  // for now
                protocols.ip_xact_export_design ww zone_filename_ip_xact master_parameters files hl_component_instances hl_connections


        // Write as RTL design
        let rtl_items =
            let downer (hli:vm2_iinfo_t) =
                let loid:verilog_hdr.verilog_cell_kind_t =
                    {
                        name=     hli.vlnv.kind
                        dims=     (0,0) // Physical dimensions
                        delays=   []    // 
                    }

                let contents = []
                let rides = []
                let contacts = []
                let atts = []
                let lli = verilog_hdr.V_INSTANCE([], loid, hli.iname, rides,  contacts, atts)
                (lli, contents)
            map downer hl_component_instances
                
        let ip_integrator_write_rtl ww =
            let msg = "ip_integrator_write_rtl"
            let ww = WF 1 msg ww (sprintf "Start write RTL file %s for zone %s" zone_filename_rtl zone.name)
            let ddctrl =
                { verilog_hdr.g_null_ddctrl with
                      uniquify_all_functions= true
                      kandr=                  false
                }
            let netinfo_dir = new verilog_hdr.netinfo_dir_t() 
            let default_timescale = "`timescale 1ns/1ns"                
            let pagewidth = 132
            let add_aux_reports=     true
            let timescale=           default_timescale
            let fvv = (pagewidth, ddctrl)
            let auxlist = []

            let ww' = (WN "HPR System Integrator write_rtl_file" ww)
            let gv_nets = []
            let lv_nets = []
            let name = op_vlnv.kind
            let newpramdefs = []
            let asrf = None // not used: if resets="asynchronous" then Some(xi_blift(reset())) else None
            let _ = verilog_render.rtl_output2 ww' zone_filename_rtl timescale netinfo_dir auxlist add_aux_reports [(asrf, name, fvv, (newpramdefs, gv_nets, lv_nets), rtl_items)]
            () 

            let ww = WF 1 msg ww (sprintf "Finish write RTL file %s for zone %s" zone_filename_rtl zone.name)

            ()
        let _ = if settings.render_rtl then ip_integrator_write_rtl ww
        ()

        
    let _ = app (write_zone ww) zones

    let ww = WF 1 msg ww "Finished"
    ()
            




let soc_inst_test ww =
    let seed = 333
    let settings =
        {
          banner=             "HPR System_Integrator"
          table_reports=      ref []
          p_concs_sortedout=  new p_concs_sortedout_t("p_concs_sortedout")
          prng=               ref seed
          dlimit=             3
          render_dot=         true  // Whether to write output graphically
          render_rtl=         true  // Whether to write output as an RTL structural netlist.
          render_ip_xact=     true  // Whether to write output as an IP-XACT design        

        }
    let _ = temporaryloadlib()
    let primary_block = "primex"
    let _ = system_integrator ww (settings:soc_inst_settings_t) primary_block 
    ()




// eof
    
