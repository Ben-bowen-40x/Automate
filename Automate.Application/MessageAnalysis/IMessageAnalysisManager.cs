using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageAnalysis;

public interface IMessageAnalysisManager
{
    Result<FileInfo> Manage<T>(string reportDefault, string messages, string calls, string customers, string report, MessageType type) where T : IConvert;
    Result<FileInfo> Manage<T>(string reportDefault, string messages, string calls, string customers, string report, bool truncate, MessageType type, int days) where T : IConvert;
}