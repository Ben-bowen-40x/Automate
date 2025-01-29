using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageAnalysis;

public interface IMessageAnalysisManager
{
    Result<FileInfo> Manage<T>(string reportDefault, string messages, string calls, string customers, string report) where T : IConvert;
    Result<FileInfo> Manage<T>(string reportDefault, string messages, string calls, string customers, string report, bool truncate, int days) where T : IConvert;
    

}