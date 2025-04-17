@echo off
title Execute daily queries

echo Please have your password ready
set /p host="Please enter the url of the database: "
set /p user="Please enter your username: "

rem Active HPP & YEP
echo Active HPP and YEP Query
set hepyep="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active HPP & YEP only.sql"
set hepyepOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\HepYep.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p -h %host% -D dwh_reportsdb < %hepyep% --batch > %hepyepOutput%
set hepyepErr=%errorlevel%

rem Active Termite
echo Active Termite Query
set termite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active Termite only.sql"
set termiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\Termite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p -h %host% -D dwh_reportsdb < %termite% --batch > %termiteOutput%
set termiteErr=%errorlevel%

rem CornFormation
echo Corn Formation Query
set cornOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\CornFormationReport.tsv
set cornQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\CornFormation.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p -h %host% -D dwh_internetmarketingdb < %cornQuery% --batch > %cornOutput%
set corn=%errorlevel%

rem GoonDoggle
echo GoonDoggle Query
set goonOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GoonDoggleReport.tsv
set goonQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GoonDoggle.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p -h %host% -D dwh_internetmarketingdb < %goonQuery% --batch > %goonOutput%
set goon=%errorlevel%

rem MacBang
echo MacBang Query
set macBangOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\MacBangReport.tsv
set macbangQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\MacBang.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p -h %host% -D dwh_internetmarketingdb < %macbangQuery% --batch > %macBangOutput%
set macBang=%errorlevel%

rem PanFries
echo Pan Fries Query
set panOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\PanFriesReport.tsv
set panQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\PanFries.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p -h %host% -D dwh_internetmarketingdb < %panQuery% --batch > %panOutput%
set pan=%errorlevel%

echo Error levels specified here
echo Corn success: %corn%
echo Corn output: %cornOutput%

echo Goon success: %goon%
echo Goon output: %goonOutput%

echo MacBang success: %macBang%
echo MacBang output: %macBangOutput%

echo Pan success: %pan%
echo Pan output: %panOutput%

echo HepYep success: %hepyepErr%
echo HepYep output: %hepyepOutput%

echo Termite success: %termiteErr%
echo Termite output: %termiteOutput%

pause