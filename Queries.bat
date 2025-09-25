@echo off
title Execute daily queries

rem Inputs from Command Line
echo Please have your password ready
set /p host="Please enter the url of the database: "
set /p user="Please enter your username: "
set /p pass="Please enter your password: "

rem Active Not Termite
echo.
echo Active not Termite query
set notTermite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active NOT Termite.sql"
set notTermiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\NotTermite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %notTermite% > %notTermiteOutput%
set notTermiteErr=%errorlevel%

rem Active Termite
echo.
echo Active Termite Query
set termite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active Termite only.sql"
set termiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\Termite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %termite% > %termiteOutput%
set termiteErr=%errorlevel%

rem CornFormation
echo.
echo Corn Formation Query
set cornOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\CornFormationReport.tsv
set cornQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\CornFormation.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %cornQuery% > %cornOutput%
set corn=%errorlevel%

rem GoonDoggle
echo.
echo GoonDoggle Query
set goonOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GoonDoggleReport.tsv
set goonQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GoonDoggle.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %goonQuery% > %goonOutput%
set goon=%errorlevel%

rem MacBang
echo.
echo MacBang Query
set macBangOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\MacBangReport.tsv
set macbangQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\MacBang.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %macbangQuery% > %macBangOutput%
set macBang=%errorlevel%

rem PanFries
echo.
echo Pan Fries Query
set panOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\PanFriesReport.tsv
set panQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\PanFries.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %panQuery% > %panOutput%
set pan=%errorlevel%

rem Lotus
echo.
echo Lotus Query
set lotusOut=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\LotusReport.tsv
set lotusQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\Lotus.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb --batch < %lotusQuery% > %lotusOut%
set lotus=%errorlevel%

rem KatharticSummary
echo.
echo KatharticSummary Query
set katharticOut=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\KatharticSummary.tsv
set katharticQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\KatharticSummary.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_ctmdb --batch < %katharticQuery% > %katharticOut%
set katharsis=%errorlevel%

rem Upsilon
echo.
echo Upsilon Query
set upsilonOut=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\UpsilonOut.tsv
set upsilonQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\Upsilon.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_ctmdb --batch < %upsilonQuery% > %upsilonOut%
set upsilon=%errorlevel%

rem Giggle
echo.
echo Giggle Custard
set custard="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GiggleCustardQuery.sql"
set custardOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GigglyCustard.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %custard% > %custardOutput%
set custardErr=%errorlevel%

rem LeafQuery
echo.
echo LeafQuery
set leafQ="%USERPROFILE%\Repos\Sql-Queries\LeafDataQuery.sql"
set leafQutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\LeafQueryOut.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb --batch < %leafQ% > %leafQutput%
set leafQErr=%errorlevel%

rem Error Levels
echo.
echo.
echo Error levels specified here
echo Not Termite success: %notTermiteErr%
echo Not Termite output: %notTermiteOutput%
rem error messages are placed in the output file
if not %notTermiteErr%==0 type %notTermiteOutput% 
echo.

echo Termite success: %termiteErr%
echo Termite output: %termiteOutput%
rem error messages are placed in the output file
if not %termiteErr%==0 type %termiteOutput% 
echo.

echo Corn success: %corn%
echo Corn output: %cornOutput%
rem error messages are placed in the output file
if not %corn%==0 type %cornOutput% 
echo.

echo Goon success: %goon%
echo Goon output: %goonOutput%
rem error messages are placed in the output file
if not %goon%==0 type %goonOutput% 
echo.

echo MacBang success: %macBang%
echo MacBang output: %macBangOutput%
rem error messages are placed in the output file
if not %macBang%==0 type %macBangOutput% 
echo.

echo Pan success: %pan%
echo Pan output: %panOutput%
rem error messages are placed in the output file
if not %pan%==0 type %panOutput% 
echo.

echo Lotus success: %lotus%
echo Lotus output: %lotusOut%
rem error messages are placed in the output file
if not %lotus%==0 type %lotusOut% 
echo.

echo KatharticSummary success: %katharsis%
echo KatharticSummary output: %katharticOu
rem error messages are placed in the output filet%
if not %katharsis%==0 type %katharticOut% 
echo.

echo Upsilon Query success: %upsilon%
echo Upsilon output: %upsilonOut%
rem error messages are placed in the output file
if not %upsilon%==0 type %upsilonOut% 
echo.

echo Custard Query success: %custardErr%
echo Custard output: %custardOutput%
rem error messages are placed in the output file
if not %custardErr%==0 type %custardOutput% 
echo.

echo Leaf Query success: %leafQErr%
echo Leaf output: %leafQutput%
rem error messages are placed in the output file
if not %leafQErr%==0 type %leafQutput% 
echo.

rem Stop if there is an error
if not "%notTermiteErr%"=="0" goto :pauseExecution
if not "%termiteErr%"=="0" goto :pauseExecution
if not "%corn%"=="0" goto :pauseExecution
if not "%goon%"=="0" goto :pauseExecution
if not "%macBang%"=="0" goto :pauseExecution
if not "%pan%"=="0" goto :pauseExecution
if not "%lotus%"=="0" goto :pauseExecution
if not "%katharsis%"=="0" goto :pauseExecution
if not "%upsilon%"=="0" goto :pauseExecution
if not "%custardErr%"=="0" goto :pauseExecution

rem Ending
echo All Executions were successful!
goto :end

:pauseExecution
echo At least one execution failed
pause

:end
timeout /t 5