@echo off
title Execute daily queries

rem Inputs from Command Line
echo Please have your password ready
set /p host="Please enter the url of the database: "
set /p user="Please enter your username: "
set /p pass="Please enter your password: "
set failedQuery="None"
echo.

rem GoonDoggle
echo GoonDoggle Query
set goonQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GoonDoggle.sql"
set goonOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GoonDoggleReport.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %goonQuery% > %goonOutput%
set goonErr=%errorlevel%
echo Goon Query success: %goonErr%
echo Goon output: %goonOutput%
rem error messages are placed in the output file
if not "%goonErr%"=="0" (
    type %goonOutput% 
    set failedQuery="GoonDoggle Query"
    goto :pauseExecution
)
echo.

rem MacBang
echo MacBang Query
set macBangQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\MacBang.sql"
set macBangOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\MacBangReport.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %macBangQuery% > %macBangOutput%
set macBangErr=%errorlevel%
echo MacBang Query success: %macBangErr%
echo MacBang output: %macBangOutput%
rem error messages are placed in the output file
if not "%macBangErr%"=="0" (
    type %macBangOutput% 
    set failedQuery="MacBang Query"
    goto :pauseExecution
)
echo.

rem PanFries
echo Pan Fries Query
set panQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\PanFries.sql"
set panOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\PanFriesReport.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %panQuery% > %panOutput%
set panErr=%errorlevel%
echo Pan Query success: %panErr%
echo Pan output: %panOutput%
rem error messages are placed in the output file
if not "%panErr%"=="0" (
    type %panOutput% 
    set failedQuery="Pan Fries Query"
    goto :pauseExecution
)
echo.

rem Lotus
echo Lotus Query
set lotusQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\Lotus.sql"
set lotusOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\LotusReport.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %lotusQuery% > %lotusOutput%
set lotusErr=%errorlevel%
echo Lotus Query success: %lotusErr%
echo Lotus output: %lotusOutput%
rem error messages are placed in the output file
if not "%lotusErr%"=="0" (
    type %lotusOutput% 
    set failedQuery="Lotus Query"
    goto :pauseExecution
)
echo.

:: rem KatharticSummary
:: echo KatharticSummary Query
:: set katharticQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\KatharticSummary.sql"
:: set katharticOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\KatharticSummary.tsv"
:: "C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_ctmdb --batch < %katharticQuery% > %katharticOutput%
:: set katharticErr=%errorlevel%
:: echo KatharticSummary Query success: %katharticErr%
:: echo KatharticSummary output: %katharticOutput%
:: rem error messages are placed in the output file
:: if not "%katharticErr%"=="0" (
::     type %katharticOutput% 
::     set failedQuery="KatharticSummary Query"
::     goto :pauseExecution
:: )
:: echo.

:: rem Upsilon
:: echo Upsilon Query
:: set upsilonQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\Upsilon.sql"
:: set upsilonOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\UpsilonOut.tsv"
:: "C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_ctmdb --batch < %upsilonQuery% > %upsilonOutput%
:: set upsilonErr=%errorlevel%
:: echo Upsilon Query success: %upsilonErr%
:: echo Upsilon output: %upsilonOutput%
:: rem error messages are placed in the output file
:: if not "%upsilonErr%"=="0" (
::     type %upsilonOutput% 
::     set failedQuery="Upsilon Query"
::     goto :pauseExecution
:: )
:: echo.

rem Giggle
echo Giggle Custard
set custardQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GiggleCustardQuery.sql"
set custardOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GigglyCustard.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %custardQuery% > %custardOutput%
set custardErr=%errorlevel%
echo Custard Query success: %custardErr%
echo Custard output: %custardOutput%
rem error messages are placed in the output file
if not "%custardErr%"=="0" (
    type %custardOutput% 
    set failedQuery="Giggle Custard"
    goto :pauseExecution
)
echo.

rem Not Giggle
echo Giggle Custard Not
set custardNotQuery="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GiggleNotCustardQuery.sql"
set custardNotOutput="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GigglyNotCustard.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %custardNotQuery% > %custardNotOutput%
set custardNotErr=%errorlevel%
echo Custard Not Query success: %custardNotErr%
echo Custard Not output: %custardNotOutput%
rem error messages are placed in the output file
if not "%custardNotErr%"=="0" (
    type %custardNotOutput% 
    set failedQuery="Giggle Custard Not"
    goto :pauseExecution
)
echo.

rem HPP
echo HPP
set hppQuery="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\HPP Recurring.sql"
set hppOutput="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\HPP Recurring.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %hppQuery% > %hppOutput%
set hppErr=%errorlevel%
echo HPP Query success: %hppErr%
echo hpp output: %hppOutput%
rem error messages are placed in the output file
if not "%hppErr%"=="0" (
    type %hppOutput%
    set failedQuery="HPP"
    goto :pauseExecution
)
echo.

rem SS
echo SS
set ssQuery="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\SS Recurring.sql"
set ssOutput="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\SS Recurring.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %ssQuery% > %ssOutput%
set ssErr=%errorlevel%
echo SS Query success: %ssErr%
echo ss output: %ssOutput%
rem error messages are placed in the output file
if not "%ssErr%"=="0" (
    type %ssOutput%
    set failedQuery="SS"
    goto :pauseExecution
)
echo.

rem TDP
echo TDP
set tdpQuery="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\TDP Recurring.sql"
set tdpOutput="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\TDP Recurring.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %tdpQuery% > %tdpOutput%
set tdpErr=%errorlevel%
echo Tdp Query success: %tdpErr%
echo tdp output: %tdpOutput%
rem error messages are placed in the output file
if not "%tdpErr%"=="0" (
    type %tdpOutput%
    set failedQuery="TDP"
    goto :pauseExecution
)
echo.

rem YEP
echo YEP
set yepQuery="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\YEP Recurring.sql"
set yepOutput="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\YEP Recurring.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %yepQuery% > %yepOutput%
set yepErr=%errorlevel%
echo YEP Query succeyep: %yepErr%
echo yep output: %yepOutput%
rem error messages are placed in the output file
if not "%yepErr%"=="0" (
    type %yepOutput%
    set failedQuery="YEP"
    goto :pauseExecution
)
echo.

rem Cxl60
echo Cxl60
set cxl60Query="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\Cxl60 Recurring.sql"
set cxl60Output="%USERPROFILE%\Repos\Sql-Queries\Code\Recurring\Cxl60 Recurring.tsv"
"C:\Program Files\MySQL\MySQL Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %cxl60Query% > %cxl60Output%
set cxl60Err=%errorlevel%
echo Cxl60 Query success: %cxl60Err%
echo Cxl60 output: %cxl60Output%
rem error messages are placed in the output file
if not "%cxl60Err%"=="0" (
    type %cxl60Output%
    set failedQuery="Cxl60"
    goto :pauseExecution
)
echo.

rem Ending
echo All Executions were successful!
goto :EOF

:pauseExecution
echo At least one execution failed
echo Failed Query: %failedQuery%
pause
