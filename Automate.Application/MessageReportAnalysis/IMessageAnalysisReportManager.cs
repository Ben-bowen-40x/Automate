using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageReportAnalysis;

public interface IMessageAnalysisReportManager
{
    Result<FileInfo> Manage<T>(string commandName, FileInfo messages, FileInfo calls, FileInfo customers, string report, MessageType type) where T : IConvert;
    Result<FileInfo> Manage<T>(string reportDefault, FileInfo messages, FileInfo callsFile, FileInfo customersFile, string report, string truncatedReport, bool truncate, MessageType type, int days) where T : IConvert;

}