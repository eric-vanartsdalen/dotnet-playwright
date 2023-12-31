﻿
This tool has been created to help use with dotnet commandline instead of powershell commands to work with Playwright cli.
It is an attempt to solve the issues mentioned in Playwright here:

REF: https://github.com/microsoft/playwright-dotnet/issues/2286

The general tutorial 'Tutorial: Create a .NET tool using the .NET CLI' was used as a guide to help build this tool.

REF: https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create

CURRENT COMANDS TO INSTALL TOOL AT ROOT DIRECTORY:
```
dotnet tool install --add-source .\src\dotnet-playwright\nupkg dotnet-playwright
```
AND UNINSTALL TOOL AT ROOT DIRECTORY:
```
dotnet tool uninstall dotnet-playwright
```

This shoudl alter the dotnet-tools.json manifest and allow you to execute Playwright commands.
```
dotnet playwright

playwright -V
```
You should also be able to just type playwright also.

LEARNINGS THAT HELPED:

When playwright versions have not been installed, a powershell script command message to install 
is typically messaged...

Note: Playwright versions co-relation specific downloaded Browser versions that Playwright itself uses... 

You might see a pwsh command output like:
```
╔════════════════════════════════════════════════════════════╗
║ Looks like Playwright was just installed or updated.       ║
║ Please run the following command to download new browsers: ║
║                                                            ║
║     pwsh bin/Debug/netX/playwright.ps1 install             ║
║                                                            ║
║ <3 Playwright Team                                         ║
╚════════════════════════════════════════════════════════════╝
```

The contents of the 'playwright.ps1' script is as follows:
```
#!/usr/bin/env pwsh

$PlaywrightFileName = Join-Path $PSScriptRoot "Microsoft.Playwright.dll"
[Reflection.Assembly]::LoadFile($PlaywrightFileName) | Out-Null
exit [Microsoft.Playwright.Program]::Main($args)
```
The script is dropped into the bin directory when the Test application is built
- it wraps commands submitted to and outputs from a DLL called 'Microsoft.Playwright.dll'

Examples of manual execution to the dll might look like:
To Install, command would be:
Microsoft.Playwright.dll install

To uninstall Playwright browsers (per version of dll referenced)
Microsoft.Playwright.dll uninstall

To uninstall all versions of Playwright browsers installed
Microsoft.Playwright.dll uninstall --all

etc...

This project is a wrapper for the Microsoft.Playwright.dll in executable form as an extension of the dotnet CLI.


It is the expectation that this be open source and helpful to someone else without charge.
This is AS-IS and no warranty is provided.
