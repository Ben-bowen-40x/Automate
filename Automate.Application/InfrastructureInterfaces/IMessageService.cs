using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IMessageService
{
    List<IMessage> GetMessages<T>(string messageLocation) where T : IConvert;
    List<ICallRecord> GetCallRecords(List<long> msgNums, string callLocation);
    List<ICustomerSubscription> GetCustomerRecords(List<long> msgNums, string customerLocation);
}