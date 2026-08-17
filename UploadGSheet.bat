@echo off

set "CREDENTIALS=%USERPROFILE%\.secrets\solid-sun-505420-j9-dcfecb92194c.json"
set "AUTO=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports"
set "SCRIPT=%~dp0upload_to_gsheet.py"
set "FOUNDERR="

rem -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
rem Execute each data file
rem -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
rem                  Data                              Sheet                                                                                    Tab
set "lotusData=%AUTO%\QueryReports\LotusReport.tsv"
call :runScript      "%lotusData%"                     "https://docs.google.com/spreadsheets/d/1AIIShc2m5OSxEMejjmzaF2iPTUsMpFmIJz7SuLHmBVk/"   "Sheet1"
call :checkFailure   "%errorlevel%"                    "%lotusData%"                                                                            "File not found: %lotusData%"

set "goondoggleData=%AUTO%\QueryReports\GoonDoggleReport.tsv"
call :runScript      "%goondoggleData%"               "https://docs.google.com/spreadsheets/d/1sPMqGHke18zHWIfAwdvZp0FEiJomVr6vWbCIbeYezL8/"    "Upload"
call :checkFailure   "%errorlevel%"                    "%goondoggleData%"                                                                       "File not found: %goondoggleData%"

set "gleafData=%AUTO%\QueryReports\LeafQueryOut.tsv"
call :runScript      "%gleafData%"                    "https://docs.google.com/spreadsheets/d/1WuzzEqA5fD3MJIPvgpzt1kJ9eHyCuKzpXdS8e8pmMXM/"    "Upload"
call :checkFailure   "%errorlevel%"                    "%gleafData%"                                                                            "File not found: %gleafData%"

set "macbangData=%AUTO%\QueryReports\MacBangReport.tsv"
call :runScript      "%macbangData%"                  "https://docs.google.com/spreadsheets/d/1OiNgo-wgm0UrSa3GuAiIcBHntt3sSwXUucxBuZCVkNg/"    "Upload"
call :checkFailure   "%errorlevel%"                    "%macbangData%"                                                                          "File not found: %macbangData%"

set "mleafData=%AUTO%\QueryReports\LeafQueryOutB.tsv"
call :runScript      "%mleafData%"                    "https://docs.google.com/spreadsheets/d/1PcV0pLtNNgMY54HGMBx9hyJAS0uUj8N_XwNdPdUFNBU/"    "Upload"
call :checkFailure   "%errorlevel%"                    "%mleafData%"                                                                            "File not found: %mleafData%"

set "panData_1=%AUTO%\QueryReports\PanFriesReport.tsv"
set "pan=https://docs.google.com/spreadsheets/d/11EGPilWqfafvGt8XGnVkQoKLAcrTHv6KAe7o2mq0h_w/"
call :runScript      "%panData_1%"                     "%pan%"                                                                                  "Calls -- Sale Values"
call :checkFailure   "%errorlevel%"                    "%panData_1%"                                                                            "File not found: %panData_1%"

set "panData_2=%AUTO%\PanReport.csv"
call :runScript      "%panData_2%"                     "%pan%"                                                                                  "All Sources Import"
call :checkFailure   "%errorlevel%"                    "%panData_2%"                                                                            "File not found: %panData_2%"

set "hemorrhoidData=%AUTO%\Hemorrhoid_Test.csv"
call :runScript      "%hemorrhoidData%"               "https://docs.google.com/spreadsheets/d/1WnGelviCwxgWdtynzop-XaBJ4xHuy76PdjMupY39Jig/"    "New ROI Sheet"
call :checkFailure   "%errorlevel%"                    "%hemorrhoidData%"                                                                       "File not found: %hemorrhoidData%"

set "yellerData=%AUTO%\QueryReports\YellerROI.tsv"
call :runScript     "%yellerData%"                    "https://docs.google.com/spreadsheets/d/1uTSbyMz65XB72OWzmx5SUN5sHv0zJq7CtUqqUqA6IeA/"    "Upload"
call :checkFailure  "%errorlevel%"                    "%yellerData%"                                                                            "File not found: %yellerData%"

rem Check for any failure at all, exit cleanly
if "%FOUNDERR%"=="TRUE" (
   echo Errors found. Please review
   pause
   exit /b 1
)
goto :eof

rem ---------------------------------------------------------------------------------------------
rem Script function
rem ---------------------------------------------------------------------------------------------
:runScript 
setlocal
set "data=%~1"
set "sheet=%~2"
set "tab=%~3"

rem Check the file for existence
echo Checking file for existence:
echo        %data%
call :checkfile "%data%"
call :checkFailure "%errorlevel%" "%data%" "File not found error"
set "notfounderr=%errorlevel%"

if not "%notfounderr%"=="0" (
   echo File not found:
   echo     %data%
   echo.
   echo.
   endlocal & exit /b 1
)

python "%SCRIPT%" --data-file "%data%" --sheet-url "%sheet%" --tab "%tab%" --credentials "%CREDENTIALS%" --value-input-option "RAW"
set "scripterr=%errorlevel%"

if not "%scripterr%"=="0" (
   echo File found. Script error:
   echo     Data: %data%
   echo     Sheet: %sheet%
   echo     Tab: %tab%
   echo.
   echo.
   endlocal & exit /b %scripterr%
)
endlocal & exit /b 0

rem ---------------------------------------------------------------------------------------------
rem Check file existence
rem ---------------------------------------------------------------------------------------------
:checkFile
setlocal 
set "file=%~1"

if not exist "%file%" (
   echo Cannot find file at:
   echo     %file%
   echo.
   echo.
   endlocal & exit /b 1
)
echo.
echo.
endlocal & exit /b 0

rem ---------------------------------------------------------------------------------------------
rem Failure function
rem ---------------------------------------------------------------------------------------------
:checkFailure
setlocal
set "error=%~1"
set "file=%~2"
set "failMsg=%~3"

if not "%error%"=="0" (
   echo FAILURE, exit code %error%
   echo        File: %file%
   echo        Failure Message: %failMsg%
   echo.
   echo.
   endlocal & set "FOUNDERR=TRUE" & exit /b %error%
)
endlocal & exit /b 0
