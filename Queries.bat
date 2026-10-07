@echo off
setlocal EnableExtensions DisableDelayedExpansion
title Execute daily queries

rem ============================================================
rem  Configuration
rem ============================================================
set "MYSQL=C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe"
set "AUTO=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info"
set "RECUR=%USERPROFILE%\Repos\Sql-Queries\Code\Recurring"
set "LOGDIR=%TEMP%\DailyQueries"

rem Credentials: host/user come from ConnectionStrings, password from DbPassword
set "SECRETS=%USERPROFILE%\.leadpipe\secrets.json"
set "CNF_SCRIPT=%~dp0Write-MysqlCnf.ps1"
rem Which ConnectionStrings entry to use (leave empty = first entry)
set "SCHEMA="

set "CNF="
set "failedQuery=None"

if not exist "%MYSQL%" (
    echo Cannot find mysql.exe at:
    echo   %MYSQL%
    goto :failed
)
if not exist "%CNF_SCRIPT%" (
    echo Cannot find helper script at:
    echo   %CNF_SCRIPT%
    goto :failed
)
if not exist "%LOGDIR%" md "%LOGDIR%" >nul 2>&1

rem ============================================================
rem  Credentials  (read from secrets.json into a temp option file)
rem ============================================================
set "CNF=%TEMP%\dq_%RANDOM%%RANDOM%.cnf"

set "schemaArg="
if defined SCHEMA set "schemaArg=-Schema "%SCHEMA%""

powershell -NoProfile -ExecutionPolicy Bypass -File "%CNF_SCRIPT%" -SecretsPath "%SECRETS%" -OutPath "%CNF%" %schemaArg%
if errorlevel 1 (
    set "failedQuery=Reading credentials from secrets.json"
    goto :failed
)

echo.
echo Connecting ...
"%MYSQL%" --defaults-extra-file="%CNF%" --batch --execute="SELECT 1" >nul 2>"%LOGDIR%\connect.err"
if errorlevel 1 (
    echo Connection test FAILED:
    type "%LOGDIR%\connect.err"
    set "failedQuery=Connection test"
    goto :failed
)
echo Connection OK.
echo.

rem ============================================================
rem  Queries      name              sql file                                        output file                                              database
rem ============================================================
call :runQuery "GoonDoggle"        "%AUTO%\Queries\GoonDoggle.sql"                  "%AUTO%\Reports\QueryReports\GoonDoggleReport.tsv"       dwh_internetmarketingdb || goto :failed
call :runQuery "Leaf"              "%AUTO%\Queries\LeafQuery.sql"                   "%AUTO%\Reports\QueryReports\LeafQueryOut.tsv"           dwh_internetmarketingdb || goto :failed
call :runQuery "Leaf B"            "%AUTO%\Queries\LeafQueryB.sql"                  "%AUTO%\Reports\QueryReports\LeafQueryOutB.tsv"          dwh_internetmarketingdb || goto :failed
call :runQuery "MacBang"           "%AUTO%\Queries\MacBang.sql"                     "%AUTO%\Reports\QueryReports\MacBangReport.tsv"          dwh_internetmarketingdb || goto :failed
call :runQuery "Pan Fries"         "%AUTO%\Queries\PanFries.sql"                    "%AUTO%\Reports\QueryReports\PanFriesReport.tsv"         dwh_internetmarketingdb || goto :failed
call :runQuery "Lotus"             "%AUTO%\Queries\Lotus.sql"                       "%AUTO%\Reports\QueryReports\LotusReport.tsv"            dwh_internetmarketingdb || goto :failed
call :runQuery "Yeller ROI"        "%AUTO%\Queries\YellerROI.sql"                   "%AUTO%\Reports\QueryReports\YellerROI.tsv"              dwh_internetmarketingdb || goto :failed
call :runQuery "Venn Diagram"      "%AUTO%\Queries\VennDiagram.sql"                 "%AUTO%\Reports\QueryReports\VennDiagram.tsv"            dwh_internetmarketingdb || goto :failed
::call :runQuery "Kathartic"         "%AUTO%\Queries\KatharticSummary.sql"            "%AUTO%\Reports\QueryReports\KatharticSummary.tsv"       dwh_ctmdb               || goto :failed
::call :runQuery "Upsilon"           "%AUTO%\Queries\Upsilon.sql"                     "%AUTO%\Reports\QueryReports\UpsilonOut.tsv"             dwh_ctmdb               || goto :failed
::call :runQuery "Giggle Custard"    "%AUTO%\Queries\GiggleCustardQuery.sql"          "%AUTO%\Reports\QueryReports\GigglyCustard.tsv"          dwh_reportsdb           || goto :failed
::call :runQuery "Giggle Not"        "%AUTO%\Queries\GiggleNotCustardQuery.sql"       "%AUTO%\Reports\QueryReports\GigglyNotCustard.tsv"       dwh_reportsdb           || goto :failed
::call :runQuery "HPP"               "%RECUR%\HPP Recurring.sql"                      "%RECUR%\HPP Recurring.tsv"                              dwh_reportsdb           || goto :failed
::call :runQuery "SS"                "%RECUR%\SS Recurring.sql"                       "%RECUR%\SS Recurring.tsv"                               dwh_reportsdb           || goto :failed
::call :runQuery "TDP"               "%RECUR%\TDP Recurring.sql"                      "%RECUR%\TDP Recurring.tsv"                              dwh_reportsdb           || goto :failed
::call :runQuery "YEP"               "%RECUR%\YEP Recurring.sql"                      "%RECUR%\YEP Recurring.tsv"                              dwh_reportsdb           || goto :failed
::call :runQuery "Cxl60"             "%RECUR%\Cxl60 Recurring.sql"                    "%RECUR%\Cxl60 Recurring.tsv"                            dwh_reportsdb           || goto :failed

rem ============================================================
rem  Success
rem ============================================================
call :cleanup
echo.
echo All executions were successful!
endlocal
exit /b 0

rem ============================================================
rem  :runQuery  name  sqlPath  outPath  database
rem ============================================================
:runQuery
setlocal
set "name=%~1"
set "sql=%~2"
set "out=%~3"
set "db=%~4"
set "err=%LOGDIR%\%~1.err"

echo [%name%]
if not exist "%sql%" (
    echo   ERROR: query file not found:
    echo     %sql%
    endlocal & set "failedQuery=%~1" & exit /b 2
)

"%MYSQL%" --defaults-extra-file="%CNF%" --default-character-set=utf8mb4 -D "%db%" --batch < "%sql%" > "%out%" 2> "%err%"
set "rc=%errorlevel%"

if not "%rc%"=="0" (
    echo   FAILED, exit code %rc%
    type "%err%"
    endlocal & set "failedQuery=%~1" & exit /b %rc%
)

rem Non-fatal warnings still land in the .err file
for %%F in ("%err%") do if %%~zF GTR 0 (
    echo   warnings:
    type "%err%"
)

for %%F in ("%out%") do (
    if %%~zF EQU 0 (
        echo   WARNING: output file is empty
    ) else (
        echo   OK -^> %out%
    )
)
echo.
endlocal & exit /b 0

rem ============================================================
rem  Cleanup / failure
rem ============================================================
:cleanup
if defined CNF if exist "%CNF%" del /q "%CNF%" >nul 2>&1
set "CNF="
exit /b 0

:failed
call :cleanup
echo.
echo At least one execution failed.
echo Failed query: %failedQuery%
echo Error logs:   %LOGDIR%
pause
endlocal
exit /b 1