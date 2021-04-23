# Installation 
- Mono
- C# compiler
- F# compiler
- dotnet sdk

 ``` 
 ubuntu@ubuntukhan:~$ sudo apt install mono-runtime
[sudo] password for ubuntu: 
Reading package lists... Done
Building dependency tree       
Reading state information... Done
The following packages were automatically installed and are no longer required:
  cryptsetup-bin cython cython3 dh-exec dh-systemd docutils-common efibootmgr gperf javahelper jq junit4 libaio-dev
  libbabeltrace-ctf-dev libbabeltrace-dev libblas3 libblkid-dev libcap-ng-dev libcryptsetup-dev libcunit1 libcunit1-dev
  libcurl4-openssl-dev libfuse-dev libfwup1 libgcrypt20-dev libgfortran4 libgmp-dev libgmpxx4ldbl libgnutls-dane0
  libgnutls-openssl27 libgnutls28-dev libgnutlsxx28 libgoogle-perftools-dev libgpg-error-dev libhamcrest-java libibverbs-dev
  libidn2-0-dev libidn2-dev libjq1 libjs-bootstrap libjs-d3 libjs-eonasdan-bootstrap-datetimepicker libjs-jquery-hotkeys
  libjs-moment libjs-mustache libjs-rickshaw libjs-sphinxdoc libjs-underscore libkeyutils-dev liblapack3 libldap2-dev
  libleveldb-dev libleveldb1v5 liblttng-ust-ctl4 liblttng-ust-dev liblttng-ust-python-agent0 liblttng-ust0 liblua5.3-dev liblz4-dev
  liblzma-dev liboath-dev liboath0 libonig4 libp11-kit-dev libpython3-all-dev librabbitmq-dev librabbitmq4 librdkafka++1
  librdkafka-dev librdkafka1 librdmacm-dev librdmacm1 libselinux1-dev libsepol1-dev libsnappy-dev libsqlite3-dev libtasn1-6-dev
  libudev-dev libunbound2 libunwind-dev liburcu-dev liburcu6 libxml2-dev libxmlsec1-dev libxmlsec1-gcrypt libxmlsec1-gnutls
  libxmlsec1-openssl libxslt1-dev libzstd-dev linux-headers-5.4.0-48-generic linux-hwe-5.4-headers-5.4.0-48
  linux-hwe-5.4-headers-5.4.0-58 linux-image-5.4.0-48-generic linux-modules-5.4.0-48-generic linux-modules-extra-5.4.0-48-generic
  lua-any lua5.1 luarocks nasm nettle-dev prometheus python-babel-localedata python-pastedeploy-tpl python3-alabaster python3-all
  python3-all-dev python3-babel python3-bcrypt python3-bs4 python3-cherrypy3 python3-click python3-colorama python3-coverage
  python3-decorator python3-docutils python3-imagesize python3-jinja2 python3-jwt python3-logutils python3-nose python3-numpy
  python3-paste python3-pastedeploy python3-pecan python3-pluggy python3-prettytable python3-py python3-roman python3-scipy
  python3-simplegeneric python3-singledispatch python3-sphinx python3-tempita python3-virtualenv python3-waitress python3-webob
  python3-webtest python3-werkzeug socat sphinx-common tox uuid-dev valgrind virtualenv xfslibs-dev xmlstarlet yasm
Use 'sudo apt autoremove' to remove them.
The following additional packages will be installed:
  ca-certificates-mono cli-common libmono-corlib4.5-cil libmono-i18n-west4.0-cil libmono-i18n4.0-cil libmono-security4.0-cil
  libmono-system-configuration4.0-cil libmono-system-security4.0-cil libmono-system-xml4.0-cil libmono-system4.0-cil mono-4.0-gac
  mono-gac mono-runtime-common mono-runtime-sgen
Suggested packages:
  libmono-i18n4.0-all libgamin0
The following NEW packages will be installed:
  ca-certificates-mono cli-common libmono-corlib4.5-cil libmono-i18n-west4.0-cil libmono-i18n4.0-cil libmono-security4.0-cil
  libmono-system-configuration4.0-cil libmono-system-security4.0-cil libmono-system-xml4.0-cil libmono-system4.0-cil mono-4.0-gac
  mono-gac mono-runtime mono-runtime-common mono-runtime-sgen
0 upgraded, 15 newly installed, 0 to remove and 20 not upgraded.
Need to get 4.726 kB of archives.
After this operation, 17,3 MB of additional disk space will be used.
Do you want to continue? [Y/n] Y
Get:1 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-corlib4.5-cil all 4.6.2.7+dfsg-1ubuntu1 [1.025 kB]
Get:2 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-system-xml4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [811 kB]
Get:3 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-system-security4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [53,3 kB]
Get:4 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-system-configuration4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [52,4 kB]
Get:5 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-system4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [707 kB]
Get:6 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-security4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [226 kB]
Get:7 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 mono-4.0-gac all 4.6.2.7+dfsg-1ubuntu1 [20,6 kB]
Get:8 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 mono-gac all 4.6.2.7+dfsg-1ubuntu1 [16,3 kB]
Get:9 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 mono-runtime-common amd64 4.6.2.7+dfsg-1ubuntu1 [211 kB]
Get:10 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 mono-runtime-sgen amd64 4.6.2.7+dfsg-1ubuntu1 [1.361 kB]
Get:11 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 mono-runtime amd64 4.6.2.7+dfsg-1ubuntu1 [12,3 kB]
Get:12 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 ca-certificates-mono all 4.6.2.7+dfsg-1ubuntu1 [15,5 kB]
Get:13 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 cli-common all 0.9+nmu1 [171 kB]
Get:14 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-i18n4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [20,5 kB]
Get:15 http://de.archive.ubuntu.com/ubuntu bionic/universe amd64 libmono-i18n-west4.0-cil all 4.6.2.7+dfsg-1ubuntu1 [23,6 kB]
Fetched 4.726 kB in 12s (402 kB/s)                   
Selecting previously unselected package libmono-corlib4.5-cil.
(Reading database ... 334668 files and directories currently installed.)
Preparing to unpack .../00-libmono-corlib4.5-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-corlib4.5-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package libmono-system-xml4.0-cil.
Preparing to unpack .../01-libmono-system-xml4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-system-xml4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package libmono-system-security4.0-cil.
Preparing to unpack .../02-libmono-system-security4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-system-security4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package libmono-system-configuration4.0-cil.
Preparing to unpack .../03-libmono-system-configuration4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-system-configuration4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package libmono-system4.0-cil.
Preparing to unpack .../04-libmono-system4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-system4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package libmono-security4.0-cil.
Preparing to unpack .../05-libmono-security4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-security4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package mono-4.0-gac.
Preparing to unpack .../06-mono-4.0-gac_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking mono-4.0-gac (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package mono-gac.
Preparing to unpack .../07-mono-gac_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking mono-gac (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package mono-runtime-common.
Preparing to unpack .../08-mono-runtime-common_4.6.2.7+dfsg-1ubuntu1_amd64.deb ...
Unpacking mono-runtime-common (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package mono-runtime-sgen.
Preparing to unpack .../09-mono-runtime-sgen_4.6.2.7+dfsg-1ubuntu1_amd64.deb ...
Unpacking mono-runtime-sgen (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package mono-runtime.
Preparing to unpack .../10-mono-runtime_4.6.2.7+dfsg-1ubuntu1_amd64.deb ...
Unpacking mono-runtime (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package ca-certificates-mono.
Preparing to unpack .../11-ca-certificates-mono_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking ca-certificates-mono (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package cli-common.
Preparing to unpack .../12-cli-common_0.9+nmu1_all.deb ...
Unpacking cli-common (0.9+nmu1) ...
Selecting previously unselected package libmono-i18n4.0-cil.
Preparing to unpack .../13-libmono-i18n4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-i18n4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Selecting previously unselected package libmono-i18n-west4.0-cil.
Preparing to unpack .../14-libmono-i18n-west4.0-cil_4.6.2.7+dfsg-1ubuntu1_all.deb ...
Unpacking libmono-i18n-west4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up cli-common (0.9+nmu1) ...
Setting up mono-4.0-gac (4.6.2.7+dfsg-1ubuntu1) ...
Setting up mono-gac (4.6.2.7+dfsg-1ubuntu1) ...
update-alternatives: using /usr/bin/gacutil to provide /usr/bin/cli-gacutil (global-assembly-cache-tool) in auto mode
Setting up mono-runtime-common (4.6.2.7+dfsg-1ubuntu1) ...
update-binfmts: warning: /usr/share/binfmts/cli: no executable /usr/bin/cli found, but continuing anyway as you request
Setting up libmono-security4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up mono-runtime-sgen (4.6.2.7+dfsg-1ubuntu1) ...
Setting up mono-runtime (4.6.2.7+dfsg-1ubuntu1) ...
update-alternatives: using /usr/bin/mono to provide /usr/bin/cli (cli) in auto mode
Setting up libmono-corlib4.5-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up ca-certificates-mono (4.6.2.7+dfsg-1ubuntu1) ...
Setting up libmono-i18n4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up libmono-system-xml4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up libmono-system4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up libmono-i18n-west4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up libmono-system-configuration4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Setting up libmono-system-security4.0-cil (4.6.2.7+dfsg-1ubuntu1) ...
Processing triggers for man-db (2.8.3-2ubuntu0.1) ...
Processing triggers for gnome-menus (3.13.3-11ubuntu1.1) ...
Processing triggers for ca-certificates (20210119~18.04.1) ...
Updating certificates in /etc/ssl/certs...
0 added, 0 removed; done.
Running hooks in /etc/ca-certificates/update.d...

done.
Updating Mono key store
Linux Cert Store Sync - version 4.6.2.0
Synchronize local certs with certs from local Linux trust store.
Copyright 2002, 2003 Motus Technologies. Copyright 2004-2008 Novell. BSD licensed.

I already trust 0, your new list has 129
Certificate added: CN=ACCVRAIZ1, OU=PKIACCV, O=ACCV, C=ES
Certificate added: C=ES, O=FNMT-RCM, OU=AC RAIZ FNMT-RCM
Certificate added: C=IT, L=Milan, O=Actalis S.p.A./03358520967, CN=Actalis Authentication Root CA
Certificate added: C=US, O=AffirmTrust, CN=AffirmTrust Commercial
Certificate added: C=US, O=AffirmTrust, CN=AffirmTrust Networking
Certificate added: C=US, O=AffirmTrust, CN=AffirmTrust Premium
Certificate added: C=US, O=AffirmTrust, CN=AffirmTrust Premium ECC
Certificate added: C=US, O=Amazon, CN=Amazon Root CA 1
Certificate added: C=US, O=Amazon, CN=Amazon Root CA 2
Certificate added: C=US, O=Amazon, CN=Amazon Root CA 3
Certificate added: C=US, O=Amazon, CN=Amazon Root CA 4
Certificate added: CN=Atos TrustedRoot 2011, O=Atos, C=DE
Certificate added: C=ES, CN=Autoridad de Certificacion Firmaprofesional CIF A62634068
Certificate added: C=IE, O=Baltimore, OU=CyberTrust, CN=Baltimore CyberTrust Root
Certificate added: C=NO, O=Buypass AS-983163327, CN=Buypass Class 2 Root CA
Certificate added: C=NO, O=Buypass AS-983163327, CN=Buypass Class 3 Root CA
Certificate added: C=SK, L=Bratislava, O=Disig a.s., CN=CA Disig Root R2
Certificate added: C=CN, O=China Financial Certification Authority, CN=CFCA EV ROOT
Certificate added: C=GB, S=Greater Manchester, L=Salford, O=COMODO CA Limited, CN=COMODO Certification Authority
Certificate added: C=GB, S=Greater Manchester, L=Salford, O=COMODO CA Limited, CN=COMODO ECC Certification Authority
Certificate added: C=GB, S=Greater Manchester, L=Salford, O=COMODO CA Limited, CN=COMODO RSA Certification Authority
Certificate added: C=FR, O=Dhimyotis, CN=Certigna
Certificate added: C=PL, O=Unizeto Technologies S.A., OU=Certum Certification Authority, CN=Certum Trusted Network CA
Certificate added: C=PL, O=Unizeto Technologies S.A., OU=Certum Certification Authority, CN=Certum Trusted Network CA 2
Certificate added: C=EU, L=Madrid (see current address at www.camerfirma.com/address), OID.2.5.4.5=A82743287, O=AC Camerfirma S.A., CN=Chambers of Commerce Root - 2008
Certificate added: C=GB, S=Greater Manchester, L=Salford, O=Comodo CA Limited, CN=AAA Certificate Services
Certificate added: O="Cybertrust, Inc", CN=Cybertrust Global Root
Certificate added: C=DE, O=D-Trust GmbH, CN=D-TRUST Root Class 3 CA 2 2009
Certificate added: C=DE, O=D-Trust GmbH, CN=D-TRUST Root Class 3 CA 2 EV 2009
Certificate added: O=Digital Signature Trust Co., CN=DST Root CA X3
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Assured ID Root CA
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Assured ID Root G2
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Assured ID Root G3
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Global Root CA
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Global Root G2
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Global Root G3
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert High Assurance EV Root CA
Certificate added: C=US, O=DigiCert Inc, OU=www.digicert.com, CN=DigiCert Trusted Root G4
Certificate added: C=TR, L=Ankara, O=E-Tuğra EBG Bilişim Teknolojileri ve Hizmetleri A.Ş., OU=E-Tugra Sertifikasyon Merkezi, CN=E-Tugra Certification Authority
Certificate added: C=ES, O=Agencia Catalana de Certificacio (NIF Q-0801176-I), OU=Serveis Publics de Certificacio, OU=Vegeu https://www.catcert.net/verarrel (c)03, OU=Jerarquia Entitats de Certificacio Catalanes, CN=EC-ACC
Certificate added: O=Entrust.net, OU=www.entrust.net/CPS_2048 incorp. by ref. (limits liab.), OU=(c) 1999 Entrust.net Limited, CN=Entrust.net Certification Authority (2048)
Certificate added: C=US, O="Entrust, Inc.", OU=www.entrust.net/CPS is incorporated by reference, OU="(c) 2006 Entrust, Inc.", CN=Entrust Root Certification Authority
Certificate added: C=US, O="Entrust, Inc.", OU=See www.entrust.net/legal-terms, OU="(c) 2012 Entrust, Inc. - for authorized use only", CN=Entrust Root Certification Authority - EC1
Certificate added: C=US, O="Entrust, Inc.", OU=See www.entrust.net/legal-terms, OU="(c) 2009 Entrust, Inc. - for authorized use only", CN=Entrust Root Certification Authority - G2
Certificate added: C=CN, O="GUANG DONG CERTIFICATE AUTHORITY CO.,LTD.", CN=GDCA TrustAUTH R5 ROOT
Certificate added: C=US, O=GeoTrust Inc., OU=(c) 2007 GeoTrust Inc. - For authorized use only, CN=GeoTrust Primary Certification Authority - G2
Certificate added: OU=GlobalSign ECC Root CA - R4, O=GlobalSign, CN=GlobalSign
Certificate added: OU=GlobalSign ECC Root CA - R5, O=GlobalSign, CN=GlobalSign
Certificate added: C=BE, O=GlobalSign nv-sa, OU=Root CA, CN=GlobalSign Root CA
Certificate added: OU=GlobalSign Root CA - R2, O=GlobalSign, CN=GlobalSign
Certificate added: OU=GlobalSign Root CA - R3, O=GlobalSign, CN=GlobalSign
Certificate added: C=EU, L=Madrid (see current address at www.camerfirma.com/address), OID.2.5.4.5=A82743287, O=AC Camerfirma S.A., CN=Global Chambersign Root - 2008
Certificate added: C=US, O="The Go Daddy Group, Inc.", OU=Go Daddy Class 2 Certification Authority
Certificate added: C=US, S=Arizona, L=Scottsdale, O="GoDaddy.com, Inc.", CN=Go Daddy Root Certificate Authority - G2
Certificate added: C=GR, L=Athens, O=Hellenic Academic and Research Institutions Cert. Authority, CN=Hellenic Academic and Research Institutions ECC RootCA 2015
Certificate added: C=GR, O=Hellenic Academic and Research Institutions Cert. Authority, CN=Hellenic Academic and Research Institutions RootCA 2011
Certificate added: C=GR, L=Athens, O=Hellenic Academic and Research Institutions Cert. Authority, CN=Hellenic Academic and Research Institutions RootCA 2015
Certificate added: C=HK, O=Hongkong Post, CN=Hongkong Post Root CA 1
Certificate added: C=US, O=Internet Security Research Group, CN=ISRG Root X1
Certificate added: C=US, O=IdenTrust, CN=IdenTrust Commercial Root CA 1
Certificate added: C=US, O=IdenTrust, CN=IdenTrust Public Sector Root CA 1
Certificate added: C=ES, O=IZENPE S.A., CN=Izenpe.com
Certificate added: C=HU, L=Budapest, O=Microsec Ltd., CN=Microsec e-Szigno Root CA 2009, E=info@e-szigno.hu
Certificate added: C=HU, L=Budapest, O=NetLock Kft., OU=Tanúsítványkiadók (Certification Services), CN=NetLock Arany (Class Gold) Főtanúsítvány
Certificate added: C=US, O=Network Solutions L.L.C., CN=Network Solutions Certificate Authority
Certificate added: C=CH, O=WISeKey, OU=OISTE Foundation Endorsed, CN=OISTE WISeKey Global Root GB CA
Certificate added: C=BM, O=QuoVadis Limited, OU=Root Certification Authority, CN=QuoVadis Root Certification Authority
Certificate added: C=BM, O=QuoVadis Limited, CN=QuoVadis Root CA 1 G3
Certificate added: C=BM, O=QuoVadis Limited, CN=QuoVadis Root CA 2
Certificate added: C=BM, O=QuoVadis Limited, CN=QuoVadis Root CA 2 G3
Certificate added: C=BM, O=QuoVadis Limited, CN=QuoVadis Root CA 3
Certificate added: C=BM, O=QuoVadis Limited, CN=QuoVadis Root CA 3 G3
Certificate added: C=US, S=Texas, L=Houston, O=SSL Corporation, CN=SSL.com EV Root Certification Authority ECC
Certificate added: C=US, S=Texas, L=Houston, O=SSL Corporation, CN=SSL.com EV Root Certification Authority RSA R2
Certificate added: C=US, S=Texas, L=Houston, O=SSL Corporation, CN=SSL.com Root Certification Authority ECC
Certificate added: C=US, S=Texas, L=Houston, O=SSL Corporation, CN=SSL.com Root Certification Authority RSA
Certificate added: C=PL, O=Krajowa Izba Rozliczeniowa S.A., CN=SZAFIR ROOT CA2
Certificate added: C=JP, O="Japan Certification Services, Inc.", CN=SecureSign RootCA11
Certificate added: C=US, O=SecureTrust Corporation, CN=SecureTrust CA
Certificate added: C=US, O=SecureTrust Corporation, CN=Secure Global CA
Certificate added: C=JP, O="SECOM Trust Systems CO.,LTD.", OU=Security Communication RootCA2
Certificate added: C=JP, O=SECOM Trust.net, OU=Security Communication RootCA1
Certificate added: C=FI, O=Sonera, CN=Sonera Class2 CA
Certificate added: C=NL, O=Staat der Nederlanden, CN=Staat der Nederlanden EV Root CA
Certificate added: C=NL, O=Staat der Nederlanden, CN=Staat der Nederlanden Root CA - G3
Certificate added: C=US, O="Starfield Technologies, Inc.", OU=Starfield Class 2 Certification Authority
Certificate added: C=US, S=Arizona, L=Scottsdale, O="Starfield Technologies, Inc.", CN=Starfield Root Certificate Authority - G2
Certificate added: C=US, S=Arizona, L=Scottsdale, O="Starfield Technologies, Inc.", CN=Starfield Services Root Certificate Authority - G2
Certificate added: C=CH, O=SwissSign AG, CN=SwissSign Gold CA - G2
Certificate added: C=CH, O=SwissSign AG, CN=SwissSign Silver CA - G2
Certificate added: C=DE, O=T-Systems Enterprise Services GmbH, OU=T-Systems Trust Center, CN=T-TeleSec GlobalRoot Class 2
Certificate added: C=DE, O=T-Systems Enterprise Services GmbH, OU=T-Systems Trust Center, CN=T-TeleSec GlobalRoot Class 3
Certificate added: C=TR, L=Gebze - Kocaeli, O=Turkiye Bilimsel ve Teknolojik Arastirma Kurumu - TUBITAK, OU=Kamu Sertifikasyon Merkezi - Kamu SM, CN=TUBITAK Kamu SM SSL Kok Sertifikasi - Surum 1
Certificate added: C=TW, O=TAIWAN-CA, OU=Root CA, CN=TWCA Global Root CA
Certificate added: C=TW, O=TAIWAN-CA, OU=Root CA, CN=TWCA Root Certification Authority
Certificate added: O=TeliaSonera, CN=TeliaSonera Root CA v1
Certificate added: C=PA, S=Panama, L=Panama City, O=TrustCor Systems S. de R.L., OU=TrustCor Certificate Authority, CN=TrustCor ECA-1
Certificate added: C=PA, S=Panama, L=Panama City, O=TrustCor Systems S. de R.L., OU=TrustCor Certificate Authority, CN=TrustCor RootCert CA-1
Certificate added: C=PA, S=Panama, L=Panama City, O=TrustCor Systems S. de R.L., OU=TrustCor Certificate Authority, CN=TrustCor RootCert CA-2
Certificate added: C=GB, O=Trustis Limited, OU=Trustis FPS Root CA
Certificate added: C=US, S=New Jersey, L=Jersey City, O=The USERTRUST Network, CN=USERTrust ECC Certification Authority
Certificate added: C=US, S=New Jersey, L=Jersey City, O=The USERTRUST Network, CN=USERTrust RSA Certification Authority
Certificate added: C=US, O="VeriSign, Inc.", OU=VeriSign Trust Network, OU="(c) 2008 VeriSign, Inc. - For authorized use only", CN=VeriSign Universal Root Certification Authority
Certificate added: C=US, OU=www.xrampsecurity.com, O=XRamp Security Services Inc, CN=XRamp Global Certification Authority
Certificate added: C=RO, O=certSIGN, OU=certSIGN ROOT CA
Certificate added: C=TW, O="Chunghwa Telecom Co., Ltd.", OU=ePKI Root Certification Authority
Certificate added: OU=GlobalSign Root CA - R6, O=GlobalSign, CN=GlobalSign
Certificate added: C=CH, O=WISeKey, OU=OISTE Foundation Endorsed, CN=OISTE WISeKey Global Root GC CA
Certificate added: C=FR, O=Dhimyotis, OU=0002 48146308100036, CN=Certigna Root CA
Certificate added: C=RO, O=CERTSIGN SA, OU=certSIGN ROOT CA G2
Certificate added: C=US, OU=emSign PKI, O=eMudhra Inc, CN=emSign ECC Root CA - C3
Certificate added: C=IN, OU=emSign PKI, O=eMudhra Technologies Limited, CN=emSign ECC Root CA - G3
Certificate added: C=US, OU=emSign PKI, O=eMudhra Inc, CN=emSign Root CA - C1
Certificate added: C=IN, OU=emSign PKI, O=eMudhra Technologies Limited, CN=emSign Root CA - G1
Certificate added: C=US, O="Entrust, Inc.", OU=See www.entrust.net/legal-terms, OU="(c) 2015 Entrust, Inc. - for authorized use only", CN=Entrust Root Certification Authority - G4
Certificate added: C=HU, L=Budapest, O=Microsec Ltd., OID.2.5.4.97=VATHU-23584497, CN=e-Szigno Root CA 2017
Certificate added: C=US, O=Google Trust Services LLC, CN=GTS Root R1
Certificate added: C=US, O=Google Trust Services LLC, CN=GTS Root R2
Certificate added: C=US, O=Google Trust Services LLC, CN=GTS Root R3
Certificate added: C=US, O=Google Trust Services LLC, CN=GTS Root R4
Certificate added: C=HK, S=Hong Kong, L=Hong Kong, O=Hongkong Post, CN=Hongkong Post Root CA 3
Certificate added: C=US, O=Microsoft Corporation, CN=Microsoft ECC Root Certificate Authority 2017
Certificate added: C=US, O=Microsoft Corporation, CN=Microsoft RSA Root Certificate Authority 2017
Certificate added: C=KR, O=NAVER BUSINESS PLATFORM Corp., CN=NAVER Global Root Certification Authority
Certificate added: C=US, S=Illinois, L=Chicago, O="Trustwave Holdings, Inc.", CN=Trustwave Global Certification Authority
Certificate added: C=US, S=Illinois, L=Chicago, O="Trustwave Holdings, Inc.", CN=Trustwave Global ECC P256 Certification Authority
Certificate added: C=US, S=Illinois, L=Chicago, O="Trustwave Holdings, Inc.", CN=Trustwave Global ECC P384 Certification Authority
Certificate added: C=CN, O=UniTrust, CN=UCA Extended Validation Root
Certificate added: C=CN, O=UniTrust, CN=UCA Global G2 Root
129 new root certificates were added to your trust store.
Import process completed.
Done
done.
Processing triggers for mime-support (3.60ubuntu1) ...
Processing triggers for desktop-file-utils (0.23-1ubuntu3.18.04.2) ...
Processing triggers for libc-bin (2.27-3ubuntu1.4) ...
ubuntu@ubuntukhan:~$ 
 ```
