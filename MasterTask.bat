@echo off
title Update repos and generate reports

call ".\Queries.bat"
call ".\ForceUpdateRepos.bat"
call ".\ReportGeneration.bat"
call ".\TrackReportChanges.bat"
