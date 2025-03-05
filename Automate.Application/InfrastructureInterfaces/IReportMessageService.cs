using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IReportMessageService
{
    public List<IMessage> RetrieveReportMessages(MessageType type, string reportLocation, out List<QualifiedMessageRecord> records);
    public List<IMessage> GetMessages<T>( string messageLocation) where T : IConvert;
    public List<IMessage> PartitionMessagesAndReportRecords(List<IMessage> uniqueMsgs, List<IMessage> reportRecords);
    public List<ICallRecord> GetCallRecords(List<long> msgNums, string callLocation);
    public List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, string customerLocation);
}
