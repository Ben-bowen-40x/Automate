@echo off
title Execute repo updates from debug
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Calls -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json"
set callRepo=%errorlevel%
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Customers -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json"
set customerRepo=%errorlevel%
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ft Leaf -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\LeafThreads.json"
set leafRepo=%errorlevel%
echo Return codes. Return code 0 indicates success
echo callRepo returned the following code: %callRepo%
echo customerRepo returned the following code: %customerRepo%
echo leafRepo returned the following code: %leafRepo%
Pause
