using Automate.Application.InfrastructureInterfaces;
using CSharpFunctionalExtensions;

namespace Automate.Application.MessageAnalysis;

public interface IMessageAnalysisManager
{
    Result<FileInfo> ManageMessageAnalysis<T>(string reportDefault, string messages, string calls, string customers, string report) where T : IMessageConvert;
}