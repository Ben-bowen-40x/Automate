@echo off
title Generate Reports 
echo Please be sure to refresh the following message lists: LeafRepo Truncated, Leased, Libacion, Pan, ManualWebForm
echo ManualWebForm needs to be downloaded, transformed into the appropriate file, then added to the below source file
Pause

rem Leaf Report
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\LeafMessages.csv" -t LeafRepo -xd 180 -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\TextReport_LF.csv" -O "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LeafReport_Gads_Truncated180.csv"
set leafrepoReport=%errorlevel%

rem Leased Report
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\LeasedMessagesInput.csv" -t Leased -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LeasedMessages.csv"
set leasedReport=%errorlevel%

rem Libacion Report
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\LibacionForm.csv " -t Libacion -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LibacionForm.csv"
set libacionReport=%errorlevel%

rem Pan Report
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\PNContactForms.csv" -t Pan -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\PanReport.csv"
set panReport=%errorlevel%

rem CalliValley
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\CalliValleyInput.csv" -t CalliValley -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\CalliValley.csv"
set calliValley=%errorlevel%

rem ContactForms 
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" convertJsonToCsv -t DwhContactForms -j "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\ContactFormsDWH2024json.json" -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\DWHforms2024.csv"
set converted=%errorlevel%

rem Ask the user to perform the final manual task
if %converted%==0 (
	echo Please place Manual web forms into the following file: 
	echo "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\ManualWebForms.csv"
) else (
	echo The conversion process was unsuccessful. 
	echo Please execute the convertJsonToCsv verb as follows ...
	echo "%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" convertJsonToCsv -t DwhContactForms -j "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\ContactFormsDWH2024json.json" -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\DWHforms2024.csv"
	echo	(That will place the web forms in the following file:
	echo	"%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\DWHforms2024.csv")
	echo ... and add the contents of the latter file into the following file manually:
	echo "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\ManualWebForms.csv" 
)
pause

rem ManualWebForms report
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c %USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json -q %USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json -as %USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\ManualWebForms.csv -t ManualWebForm -o %USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\ManualWebForms.csv
set webFormReport=%errorlevel%

echo Return codes. Return code 0 indicates success. Any other code indicates failure.
echo leafReport generation returned the following code: %leafrepoReport%
echo leasedReport generation returned the following code: %leasedReport%
echo libacionReport returned the following code: %libacionReport%
echo panReport returned the following code: %panReport%
echo webFormReport returned the following code: %webFormReport%
echo CalliValley returned the following code: %calliValley%
pause