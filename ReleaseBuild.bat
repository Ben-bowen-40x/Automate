@echo off
title Update Release build
echo Ensure that the release build is up-to-date with debug

cd %USERPROFILE%\Repos\Automate
dotnet build --configuration Release
set built=%errorlevel%

echo Were there execution errors?
echo %built%

if not "%built%"=="0" goto :pauseExecution

echo Successfully built Release!
goto :end

:pauseExecution
echo Build failure
pause

:end
timeout \t 5
echo.