using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageReportAnalysis;

public interface IMessageAnalysisReportManager
{
    Result<FileInfo> Manage<T>(string commandName, string messages, string calls, string customers, string report) where T : IConvert;
    Result<FileInfo> Manage<T>(string reportDefault, string messages, string callsFile, string customersFile, string report, string truncatedReport, bool truncate, int days) where T : IConvert;

}