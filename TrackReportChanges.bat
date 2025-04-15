@echo off
title Track Report changes with local git

echo Saving changes to tracked files

cd "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\"
git commit -am "Update Report on %date% %time%"
set success=%errorlevel%

echo Success code: %success%
cd %USERPROFILE%
pause