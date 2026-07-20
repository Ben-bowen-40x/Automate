@echo off
setlocal enabledelayedexpansion
title Update repos and generate reports

:: Record master start time
set "master_start=%TIME%"

:: Queries
set "start=%TIME%"
call ".\Queries.bat"
set queries=%errorlevel%
call :CalculateTime "%start%" "%TIME%" queries_time

:: Release Build
set "start=%TIME%"
call ".\ReleaseBuild.bat"
set build=%errorlevel%
call :CalculateTime "%start%" "%TIME%" build_time

:: Lead Pipe
set "start=%TIME%"
call ".\LeadPipe.bat"
set leadPipe=%errorlevel%
call :CalculateTime "%start%" "%TIME%" leadPipe_time

:: All Report
set "start=%TIME%"
call ".\AllReport.bat"
set allreport=%errorlevel%
call :CalculateTime "%start%" "%TIME%" allreport_time

:: Temp ROI
set "start=%TIME%"
call "%USERPROFILE%\Repos\Sql-Queries\ROI Report\Temporary ROI\ManualBatch.bat"
set manualBatch=%errorlevel%
call :CalculateTime "%start%" "%TIME%" manualBatch_time

:: Excel Open
set "start=%TIME%"
call ".\OpenFiles.bat"
set excelOpen=%errorlevel%
call :CalculateTime "%start%" "%TIME%" excelOpen_time

:: Leaf Exclusion
set "start=%TIME%"
call ".\LeafExclusion.bat"
set exclusion=%errorlevel%
call :CalculateTime "%start%" "%TIME%" exclusion_time

:: Track Report Changes
set "start=%TIME%"
call ".\TrackReportChanges.bat"
set tracking=%errorlevel%
call :CalculateTime "%start%" "%TIME%" tracking_time

:: Record master end time
call :CalculateTime "%master_start%" "%TIME%" total_time


:: ==============================================================================
:: Results Output
:: ==============================================================================
echo ==============================================================================
echo                           EXECUTION RESULTS & METRICS
echo ==============================================================================
echo.

echo [Queries] 
echo   - Errors: %queries%
echo   - Elapsed Time: %queries_time%
echo.

echo [Release Build] 
echo   - Errors: %build%
echo   - Elapsed Time: %build_time%
echo.

echo [Lead Pipe] 
echo   - Errors: %leadPipe%
echo   - Elapsed Time: %leadPipe_time%
echo.

echo [All Report] 
echo   - Errors: %allreport%
echo   - Elapsed Time: %allreport_time%
echo.

echo [TempROI] 
echo   - Errors: %manualBatch%
echo   - Elapsed Time: %manualBatch_time%
echo.

echo [Excel Opening/Saving] 
echo   - Errors: %excelOpen%
echo   - Elapsed Time: %excelOpen_time%
echo.

echo [Exclusion Execution] 
echo   - Errors: %exclusion%
echo   - Elapsed Time: %exclusion_time%
echo.

echo [Tracking Report Changes] 
echo   - Errors: %tracking%
echo   - Elapsed Time: %tracking_time%
echo.

echo ==============================================================================
echo Total Script Execution Time: %total_time%
echo ==============================================================================
echo.
pause
exit /b


:: ==============================================================================
:: Time Calculation Subroutine
:: ==============================================================================
:CalculateTime
set "start_time=%~1"
set "end_time=%~2"

:: Parse start time
for /f "tokens=1-4 delims=:.," %%a in ("%start_time%") do (
    set /a "start_h=100%%a %% 100", "start_m=100%%b %% 100", "start_s=100%%c %% 100"
)
:: Parse end time
for /f "tokens=1-4 delims=:.," %%a in ("%end_time%") do (
    set /a "end_h=100%%a %% 100", "end_m=100%%b %% 100", "end_s=100%%c %% 100"
)

:: Convert both to total seconds
set /a "start_total=(start_h * 3600) + (start_m * 60) + start_s"
set /a "end_total=(end_h * 3600) + (end_m * 60) + end_s"

:: Handle midnight crossover
if %end_total% LSS %start_total% set /a "end_total+=86400"

:: Calculate duration in seconds
set /a "duration=end_total - start_total"

:: Format into Hours, Minutes, Seconds
set /a "dur_h=duration / 3600", "dur_m=(duration %% 3600) / 60", "dur_s=duration %% 60"

:: Output the formatted string back to the requested variable
set "%~3=%dur_h%h %dur_m%m %dur_s%s"
exit /b
