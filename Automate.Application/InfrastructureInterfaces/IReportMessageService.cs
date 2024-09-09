using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IReportMessageService
{
    public List<IMessage> RetrieveReportMessages(string reportLocation, out List<QualifiedMessageRecord> records);
    public List<IMessage> GetMessages<T>(string messageLocation) where T : IMessageConvert;
    public List<IMessage> PartitionMessagesAndReportRecords(List<IMessage> uniqueMsgs, List<IMessage> reportRecords);
    public List<ICallRecord> GetCallRecords(string callLocation);
    public List<ICustomerSubscription> GetCustomerRecords(string customerLocation);
}
