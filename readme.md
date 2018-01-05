# Architecture for Control Networks (ACN)

## Project Description
Architecture for Control Networks (ACN) is a network control protocol which is used in the entertainment industry. 

This open source project aims to provide a full implementation of this standard and many of the sub protocols associated with ACN.

## Introduction

The ACN protocol is a suite of standards being developed by the lighting industry for control of lighting fixtures and other devices used by the entertainment industry. The first release of the protocol is ANSI E1.17 and other stndards have been released which form part of the suite. You can find out more about ACN on wikipedia or on the PLASA Technical Standardswebsite.

This implementation of ACN is intended to be used by projects which wish to add ACN capability to their products. The code is written in C# and fully managed.

## Supported Standards

This project is under continuous development and further protocols will be added. Here is a list of the current support provided by this library.

| Protocol Name | Standard | Implementation Status | Comments
| --- | --- |
| Root Layer Protocol (RLP) | ANSI E1.17 | Complete	
| Session Data Transport Protocol (SDT) | ANSI E1.17 | Not Implemented | Coming soon...
| Service Location Protocol (SLP) |	RFC 2609	Complete	Directory Agent Not Included
| Simple Network Time Protocol (SNTP) |	RFC 2030 ANSI E1.30-3 - 2009	Complete	
| Trivial File Transfer Protocol (TFTP) | RFC 1350	Not Implemented	
| Device Description Language (DDL) | ANSI E1.17	Not Implemented	
| Device Management Protocol (DMP) | ANSI E1.17	Partial	Only parts required for SACN have been completed.
| Streaming ACN (sACN) | ANSI E1.31 - 2016 | Complete | Updated to E1.31 2016	
| RDM Extension (RDMNet) | ANSI E1.33 |	Complete | Draft Edition of Standard, should only use for prototyping purposes!
| Remote Device Management (RDM) | ANSI E1.20 | Complete|	

## Samples

As well as implementing the ACN protocol this project also includes some samples which demonstrate the use of the library and provide tools for testing ACN,RDM and ArtNet devices. Even if you are not intending to use the library these tools may prove useful when developing ACN devices. Here is a list of the samples available.

| Name | Description
| --- | --- |
| RDM Snoop	| Discovers RDM devices and gives full control over the devices through ArtNet or RDMNet. Provides a packet log of all RDM traffic. Ideal for setting up RDM devices or diagnosing RDM faults.
| Streaming ACN Snoop | A simple application for sending and recieving sACN data.
| Sandbox ACN Device | A configurable ACN dummy device for testing purposes.
| SLP Discovery | A simple SLP discovery agent for testing and diagnostic purposes.
| SntpServer | An application based SNTP server.
| SntpUpdate | A command line util to query a single NTP or SNTP host and optionally update the system time.
