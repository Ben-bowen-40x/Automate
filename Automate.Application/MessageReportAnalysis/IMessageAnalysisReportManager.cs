using Automate.Application.InfrastructureInterfaces;

namespace Automate.Application.MessageReportAnalysis;

public interface IMessageAnalysisReportManager
{
    Dictionary<bool, FileInfo> ManageMessageAnalysis<T>(string messages, string calls, string customers, string report) where T : IMessageConvert;
}