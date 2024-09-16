using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageReportAnalysis;

public interface IMessageAnalysisReportManager
{
    Result<FileInfo> ManageMessageAnalysis<T>(string commandName, string messages, string calls, string customers, string report) where T : IMessageConvert;
}