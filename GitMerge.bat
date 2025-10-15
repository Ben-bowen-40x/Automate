::@echo off
title Merge git branches from dev

:: Switch to the correct directory
cd %USERPROFILE%\Repos\Automate

:: Checkout main
git checkout main

:: Merge dev into main
git merge dev -m "Batch File Merge on %date% %time%"

:: Push changes 
git push

:: Switch to test
git checkout test

:: merge dev into test
git merge test -m "Match File Merge on %date% %time%"

:: push changes
git Push

:: checkout into dev again
git checkout dev

:: push any changes
git push

:end
timeout /t 5
echo.