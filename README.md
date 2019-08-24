# Remote-Package-Dependency-Analysis

This is a C# based application. This project mainly focuses on working of a client-server application.
In this project, you will about understand the asynchronous message passing communication using Windows Communication Foundation Framework.
For building this project, I have used following packages.
 1) Comm : The Comm package implements asynchronous message passing communication using the Windows Communication Foundation Framework (WCF), which provides a well-engineered set of communication functionalities wrapping sockets and windows IPC. You will probably find that this communication facility will need to be factored into several packages. That is left up to you to design.
 2) Server : A package residing on a remote3 machine that exposes an HTTP endpoint for Comm Channel connections.
 The Server implements all the functionalities developed in Project #3.
 3) Client :A package, based on Windows Presentation Foundation (WPF), residing on the local machine. 
 This package provides facilities for connecting a channel to the remote Server. 
 This package provides the capabilitiy for sending requests messages for each of the functionalities of Project #3, 
 and for receiving messages with the results, and displaying the resulting information.
 
 A typical application of remote code analysis is for Code Repositories. 
 For that, Quality Assurance staff will run analyses on code in a remote repository from clients on their desktops.
 Also, developers will analyze code, written by other developers, that they need for their own work.
 
 
 # Install the project in your system 
 You will need Visual Studio 2015.
 Should use the .Net System.IO and System.Text for all I/O.
 The project consists of run.bat and compile.bat files. Run.bat - Runs the project. It is required for C# or VB.NET applications.
 Compile.bat - Compiles the project automatically. Execute run.bat and compile.bat on your command propmpt to run this project.
