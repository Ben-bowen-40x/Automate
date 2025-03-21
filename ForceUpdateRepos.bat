rem Execute repo updates from debug
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Calls -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json"
set callRepo=%errorlevel%
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ut Customers -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json"
set customerRepo=%errorlevel%
"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" updateRepo -ft Leaf -a "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\LeafThreads.json"
set leafRepo=%errorlevel%
set failure=%callRepo% neq 0 || %customerRepo% neq 0 || %leafRepo% neq 0
echo Whether there was a failure: %failure%
rem Retrieve reports
if %failure%==false (
	echo Please be sure to refresh the following message lists: LeafRepo Truncated, Leased, Libacion, Pan, Web Forms
	echo Press enter when ready to proceed with these reports
	Pause
	"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\LeafMessages.csv" -t LeafRepo -xd 180 -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\TextReport_LF.csv" -O "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LeafReport_Gads_Truncated180.csv"
	set leafrepoReport=%errorlevel%
	echo %leafrepoReport%
	"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\LeasedMessagesInput.csv" -t Leased -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LeasedMessages.csv"
	set leasedReport=%errorlevel%
	echo %leasedReport%
	"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\LibacionForm.csv " -t Libacion -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LibacionForm.csv"
	set libacionReport=%errorlevel%
	echo %libacionReport%
	"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\PNContactForms.csv" -t Pan -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\PanReport.csv"
	set panReport=%errorlevel%
	echo %panReport%
	"%USERPROFILE%\Repos\Automate\Automate.Cli\bin\Debug\net8.0\Automate.Cli.exe" analyzeMessages -c "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CallRepo.json" -q "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\ApiRepos\CustomerRepo.json" -as "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\MessageAnalysis\ManualWebForms.csv" -t ManualWebForm -o "%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\ManualWebForms.csv"
	set webFormReport=%errorlevel%
	echo %webFormReport%
	if %leafrepoReport% neq 0 || %leasedReport% neq 0 || %libacionReport% neq 0 || %panReport% neq 0 (
		echo There were failures. Please review them and process manually
	)
)
else (
	echo There was a failure somewhere in the repo updates:
	echo CallRepo execution error code: %callRepo%
	echo Customer Repo execution error code: %customerRepo%
	echo Leaf Repo execution error code: %leafRepo%
)
Pause
