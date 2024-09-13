using Automate.Application.InfrastructureInterfaces;

namespace Automate.Application.MessageAnalysis;

public interface IMessageAnalysisManager
{
    Dictionary<bool, FileInfo> ManageMessageAnalysis<T>(string reportDefault, string messages, string calls, string customers, string report) where T : IMessageConvert;
}