# CBG Orangepath.  hpr/Makefile.inc  - toolchain setup for mono and FSharp.
# HPR L/S Logic Synthesis/Formal/Compiler/Codesign System - FSharp Version.
# (C) 2006-15 DJ Greaves. All rights reserved.

# We will try to put all the machine-specific details in this setup file.
# All are defined with ?= so can be overriden with env variables instead of editing this file.

V?=0
ifeq ($(V),0)
  Q=@
else
  Q=
endif


# Mono:
# sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
# echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
# sudo apt-get update
# sudo apt-get install mono-complete fsharp

# If you do not define mono on recent linux systems, on encountering a .exe the shell will start wine and try to open windows and so on ...
MONO            ?=mono


# If you get 'bad magic number 542' on all mono programs that try to access the Console, try export TERM=xterm. 

FSC_BASIC_FLAGS ?=/nowarn:75 /consolecolors- /nologo  /lib:. --nowarn:25,64 

# Flags for additional debugging: --define:XI_ASSOC_TRACE_ENABLE


# fsharp compiler: you may have one installed already in /usr/bin/fsharpc
# Get F# aka FSharp aka fsharpc on Fedora:
# yum install yum-utils
# rpm --import "http://keyserver.ubuntu.com/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF"
# yum-config-manager --add-repo http://download.mono-project.com/repo/centos/
# yum update
# yum install mono-complete fsharp


# -g - MDB files not created when compiling on mono.



# Did link of 4.5 to 4.0 in /usr/lib/mono and used the bootstrap fsc 
# FSC= mono   /home/djg11/d320/hprls/bitbucket-hprls2/fsharp/lib/bootstrap/4.0/fsc.exe $(FSC_BASIC_FLAGS)
FSC             ?=fsharpc $(FSC_BASIC_FLAGS)  
#    --define:NNTRACE --define:XI_ASSOC_TRACE_ENABLE
# 

# 

# Mono CSharp compiler (mcs)
CSC  ?=mcs
# -g

# C++ compiler for SystemC output modes (and perhaps verilator?)
CCP  ?=g++

#--------------------------------------------------------------

# For a recent version of mono on Fedora use
# yum install yum-utils
# rpm --import "http://keyserver.ubuntu.com/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF"
# yum-config-manager --add-repo http://download.mono-project.com/repo/centos/
# yum remove mono
# yum install mono fsharp


#--------------------------------------------------------------
# fsyacc (not needed for Kiwi):
# You might get fsyacc installed on linux via nugent if you are lucky: dnf install nuget.
# or perhaps
#  1. wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
#  2. Apply mono to the build/fsyacc.exe file but I don't know what other args to give.
# If nuget fails on out-of-date certificates, do something like cert-sync /etc/pki/tls/certs/ca-bundle.crt 

# OR

# Manually: choose a download folder such as /usr/local/fsyacc/7.0.6-manual and unzip it, and manually compile the two dll's needed.
# The fsyacc tool prebuilt version is sufficient (so it does not need to be compiled locally).
#  0. mkdir -p /usr/local/fsyacc/7.0.6-manual
#  1. cd /usr/local/fsyacc/7.0.6-manual
#  2. Do a wget From https://www.nuget.org/packages/FsLexYacc/ get fslexyacc.7.0.6.nupkg or similar.
#  3. unzip fslexyacc.7.0.6.nupkg
#  4. cd $(FSYACC_DOWNLOAD_SITE)/src/fslex  ; $(FSC) --target:library Lexing.fs
#  5. cd $(FSYACC_DOWNLOAD_SITE)/src/fsyacc ; $(FSC) --target:library -r:../fslex/Lexing.dll Parsing.fs

FSYACC_DOWNLOAD_SITE ?= $(HPRLS)/FsLexYacc.7.0.6
FSYACC               ?= $(FSYACC_DOWNLOAD_SITE)/build/fsyacc.exe -v



#---
# OLD INSTRUCTIONS
# FsYacc is now a separate tool, not part of the FSharp compiler, so download from here:
# mkdir -p ~/d510/fsyacc
# cd ~/d510/fsyacc
# git clone https://github.com/fsprojects/FsLexYacc
# cd FsLexYacc
# ./build.sh


#==============================================

# Use Icarus Verilog Simuator (verilator is faster and used for big RTL SIMS)
VERILOG=iverilog
VERILATOR_ROOT ?=/usr/share/verilator

# eof
