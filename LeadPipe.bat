@echo off
title Lead Pipeline

:: Keep secrets up to date in case of changes
copy /Y "%USERPROFILE%\AppData\Roaming\Microsoft\UserSecrets\f61f648e-c555-4854-ab9a-40533affb5d6\secrets.json" "%USERPROFILE%\.leadpipe\secrets.json"

:: Execute Catman
"%USERPROFILE%\Repos\LeadPipe\LeadPipe.Cli\bin\Release\net10.0\LeadPipe.Cli.exe" catman -r
set catmanErr=%errorlevel%

:: Execute data. Data is not reliant on catman in order to run
"%USERPROFILE%\Repos\LeadPipe\LeadPipe.Cli\bin\Release\net10.0\LeadPipe.Cli.exe" data -r
set dataErr=%errorlevel%

:: Report any errors
echo.
if %catmanErr% neq 0 echo Catman had an error: %catmanErr%
if %dataErr% neq 0 echo Data update had an error: %dataErr%

:: Pause only if there was an error, otherwise exit cleanly
if %catmanErr% neq 0 goto :pauseAndExit
if %dataErr% neq 0 goto :pauseAndExit
goto :EOF

:pauseAndExit
echo.
pause