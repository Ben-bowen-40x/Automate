@echo off
title Execute repo updates from debug
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Calls -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json"
set callRepo=%errorlevel%
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Customers -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json"
set customerRepo=%errorlevel%
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Leaf -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\LeafThreads.json"
set leafRepo=%errorlevel%
echo Return codes. Return code 0 indicates success
echo	CallRepo returned the following code: %callRepo%
echo	CustomerRepo returned the following code: %customerRepo%
echo	LeafRepo returned the following code: %leafRepo%
echo We will now execute the report generation. In the case where this is unacceptable because of the return codes above, press Ctrl + c and then confirm execution closure
call ".\ReportGeneration.bat"
