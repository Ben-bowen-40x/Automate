@echo off
title Update repos and generate reports

call ".\ForceUpdateRepos.bat"
call ".\ReportGeneration.bat"
call ".\TrackReportChanges.bat"
