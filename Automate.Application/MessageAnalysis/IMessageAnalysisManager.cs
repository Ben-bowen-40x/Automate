using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageAnalysis;

public interface IMessageAnalysisManager
{
    Result<FileInfo> Manage<T>(string reportDefault, FileInfo messages, FileInfo calls, FileInfo customers, string report, MessageType type) where T : IConvert;
    Result<FileInfo> Manage<T>(string reportDefault, FileInfo messages, FileInfo calls, FileInfo customers, string report, bool truncate, MessageType type, int days) where T : IConvert;
}