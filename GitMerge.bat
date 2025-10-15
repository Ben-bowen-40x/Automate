@echo off
setlocal

rem Define the repository path and the branches to merge
set "repo_path=%USERPROFILE%\Repos\Automate"
set "dev_branch=dev"
set "main_branch=main"
set "test_branch=test"

rem --- Error Handling and Function Reusability ---

rem Function to check for errors and exit if a command fails
:check_error
if %errorlevel% neq 0 (
    echo.
    echo ERROR: Last command failed with errorlevel %errorlevel%. Exiting.
    echo.
    endlocal
    timeout /t 5 >nul
    exit /b %errorlevel%
)
goto :eof

rem Reusable function for merging and pushing a branch
:merge_and_push
set "target=%1"
set "source=%2"

echo.
echo =========================================================
echo Merging %source% into %target%...
echo =========================================================

rem Fetch all latest changes from the remote
git fetch
call :check_error

rem Checkout the target branch
git checkout %target%
call :check_error

rem Pull the latest changes for the target branch
git pull --ff-only
call :check_error

rem Merge the source branch into the target branch
git merge %source% -m "Batch File Merge: %source% into %target% on %date% %time%"
call :check_error

rem Push the changes
git push
call :check_error

echo Merge and push to %target% completed successfully.
goto :eof

rem --- Main script flow ---

echo Initializing Git merge process...
cd "%repo_path%" || (echo ERROR: Repository path not found. Exiting. && endlocal && exit /b 1)

rem Execute the merges using the function
call :merge_and_push %main_branch% %dev_branch%
call :merge_and_push %test_branch% %dev_branch%

rem Return to the original dev branch
echo.
echo Returning to the %dev_branch% branch...
git checkout %dev_branch%
call :check_error

rem Push any final changes from dev, if needed
echo Pushing final changes from %dev_branch%...
git push
call :check_error

echo.
echo All merge operations completed successfully.
echo.

:end
endlocal
timeout /t 5 >nul
