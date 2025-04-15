@echo off
title Execute repo updates from debug

"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Calls -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json"
set callRepo=%errorlevel%

"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Customers -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json"
set customerRepo=%errorlevel%

"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Leaf -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\LeafThreads.json"
set leafRepo=%errorlevel%

rem "%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateDwhRepo -t GoonDoggle -o ..\\..\\..\\..\\..\\Automate\\Automate.Infrastructure\\.info\\Reports\\GoonDoggle.csv															    
rem set goonDoggle=%errorlevel%

rem "%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateDwhRepo -t MacBang -o ..\\..\\..\\..\\..\\Automate\\Automate.Infrastructure\\.info\\Reports\\MacBang.csv
rem set macBang=%errorlevel%

rem "%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateDwhRepo -t PanFries -o ..\\..\\..\\..\\..\\Automate\\Automate.Infrastructure\\.info\\Reports\\PanFries.csv
rem set panFries=%errorlevel%

rem "%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateDwhRepo -t CornFormation -o ..\\..\\..\\..\\..\\Automate\\Automate.Infrastructure\\.info\\MessageAnalysis\\CornFormation.csv
rem set cornFormation=%errorlevel%

echo Return codes. Return code 0 indicates success
echo	CallRepo returned the following code: %callRepo%
echo	CustomerRepo returned the following code: %customerRepo%
echo	LeafRepo returned the following code: %leafRepo%
rem echo	goonDoggle returned the following code: %goonDoggle%
rem echo	macBang returned the following code: %macBang%
rem echo	panFries returned the following code: %panFries%
rem echo	cornFormation returned the following code: %cornFormation%
pause
