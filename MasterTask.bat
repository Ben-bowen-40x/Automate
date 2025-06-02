@echo off
title Update repos and generate reports

call ".\Queries.bat"
set queries=%errorlevel%
call ".\ForceUpdateRepos.bat"
set repos=%errorlevel%
call ".\ReportGeneration.bat"
set reportGen=%errorlevel%
call ".\TrackReportChanges.bat"
set tracking=%errorlevel%

echo Were there errors in the queries?
echo %queries%

echo Were there errors in the repo updates?
echo %repos%

echo Were there errors in the report generation?
echo %reportGen%

echo Were there errors in tracking report changes?
echo %tracking%

pause