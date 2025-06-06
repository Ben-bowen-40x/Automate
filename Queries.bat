@echo off
title Execute daily queries

rem Inputs from Command Line
echo Please have your password ready
set /p host="Please enter the url of the database: "
set /p user="Please enter your username: "
set /p pass="Please enter your password: "

rem Active Not Termite
echo:
echo Active not Termite query
set notTermite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active NOT Termite.sql"
set notTermiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\NotTermite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb < %notTermite% --batch > %notTermiteOutput%
set notTermiteErr=%errorlevel%

rem Active Termite
echo:
echo Active Termite Query
set termite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active Termite only.sql"
set termiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\Termite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb < %termite% --batch > %termiteOutput%
set termiteErr=%errorlevel%

rem CornFormation
echo:
echo Corn Formation Query
set cornOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\CornFormationReport.tsv
set cornQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\CornFormation.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %cornQuery% --batch > %cornOutput%
set corn=%errorlevel%

rem GoonDoggle
echo:
echo GoonDoggle Query
set goonOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GoonDoggleReport.tsv
set goonQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GoonDoggle.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %goonQuery% --batch > %goonOutput%
set goon=%errorlevel%

rem MacBang
echo:
echo MacBang Query
set macBangOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\MacBangReport.tsv
set macbangQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\MacBang.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %macbangQuery% --batch > %macBangOutput%
set macBang=%errorlevel%

rem PanFries
echo:
echo Pan Fries Query
set panOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\PanFriesReport.tsv
set panQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\PanFries.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %panQuery% --batch > %panOutput%
set pan=%errorlevel%

rem Lotus
echo:
echo Lotus Query
set lotusOut=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\LotusReport.tsv
set lotusQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\Lotus.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %lotusQuery% --batch > %lotusOut%
set lotus=%errorlevel%

rem Error Levels
echo:
echo Error levels specified here
echo Not Termite success: %notTermiteErr%
echo Not Termite output: %notTermiteOutput%
if not %notTermiteErr%==0 type %notTermiteOutput% rem error messages are placed in the output file
echo:

echo Termite success: %termiteErr%
echo Termite output: %termiteOutput%
if not %termiteErr%==0 type %termiteOutput% rem error messages are placed in the output file
echo:

echo Corn success: %corn%
echo Corn output: %cornOutput%
if not %corn%==0 type %cornOutput% rem error messages are placed in the output file
echo:

echo Goon success: %goon%
echo Goon output: %goonOutput%
if not %goon%==0 type %goonOutput% rem error messages are placed in the output file
echo:

echo MacBang success: %macBang%
echo MacBang output: %macBangOutput%
if not %macBang%==0 type %macBangOutput% rem error messages are placed in the output file
echo:

echo Pan success: %pan%
echo Pan output: %panOutput%
if not %pan%==0 type %panOutput% rem error messages are placed in the output file
echo:

echo Lotus success: %lotus%
echo Lotus output: %lotusOut%
if not %lotus%==0 type %lotusOut% rem error messages are placed in the output file
echo:

rem Stop if there is an error
if not "%notTermiteErr%"=="0" goto :pauseExecution
if not "%termiteErr%"=="0" goto :pauseExecution
if not "%corn%"=="0" goto :pauseExecution
if not "%goon%"=="0" goto :pauseExecution
if not "%macBang%"=="0" goto :pauseExecution
if not "%pan%"=="0" goto :pauseExecution
if not "%lotus%"=="0" goto :pauseExecution

rem Ending
echo All Executions were successful!
goto :end

:pauseExecution
echo At least one execution failed
pause

:end
timeout /t 5