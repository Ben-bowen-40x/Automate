@echo off
title Execute daily queries

echo Please have your password ready
set /p host="Please enter the url of the database: "
set /p user="Please enter your username: "
set /p pass="Please enter your password: "

rem Active Not Termite
echo Active not Termite query
set notTermite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active NOT Termite.sql"
set notTermiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\NotTermite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb < %notTermite% --batch > %notTermiteOutput%
set notTermiteErr=%errorlevel%

rem Active Termite
echo Active Termite Query
set termite="%USERPROFILE%\Repos\Sql-Queries\Current Customers by Service type\Active Termite only.sql"
set termiteOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\Termite.tsv
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_reportsdb < %termite% --batch > %termiteOutput%
set termiteErr=%errorlevel%

rem CornFormation
echo Corn Formation Query
set cornOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\CornFormationReport.tsv
set cornQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\CornFormation.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %cornQuery% --batch > %cornOutput%
set corn=%errorlevel%

rem GoonDoggle
echo GoonDoggle Query
set goonOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\GoonDoggleReport.tsv
set goonQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\GoonDoggle.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %goonQuery% --batch > %goonOutput%
set goon=%errorlevel%

rem MacBang
echo MacBang Query
set macBangOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\MacBangReport.tsv
set macbangQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\MacBang.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %macbangQuery% --batch > %macBangOutput%
set macBang=%errorlevel%

rem PanFries
echo Pan Fries Query
set panOutput=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\QueryReports\PanFriesReport.tsv
set panQuery=%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Queries\PanFries.sql
"C:\Program Files\MySQL\MySQl Workbench 8.0 CE\mysql.exe" -u %user% -p%pass% -h %host% -D dwh_internetmarketingdb < %panQuery% --batch > %panOutput%
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

echo HepYep success: %notTermiteErr%
echo HepYep output: %notTermiteOutput%

echo Termite success: %termiteErr%
echo Termite output: %termiteOutput%

pause