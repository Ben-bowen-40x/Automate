@echo off
title Execute repo updates from debug
setlocal

"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Release\net8.0\Automate.Cli.exe" updateRepo -ut Calls -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json"
set callRepo=%errorlevel%
echo.

"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Release\net8.0\Automate.Cli.exe" updateRepo -ut Customers -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json"
set customerRepo=%errorlevel%
echo.

call ".\LeafRepo.bat"
set leafRepo=%errorlevel%
echo.

echo Return codes. Return code 0 indicates success
echo	CallRepo returned the following code: %callRepo%
echo	CustomerRepo returned the following code: %customerRepo%
echo	LeafRepo returned the following code: %leafRepo%

if not "%callRepo%"=="0" goto :pauseExecution
if not "%customerRepo%"=="0" goto :pauseExecution
if not "%leafRepo%"=="0" goto :pauseExecution

echo All commands succeeded. Continuing...
echo.
echo.
goto :end

:pauseExecution
echo At least one command failed
pause

:end
endlocal