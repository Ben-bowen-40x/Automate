using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IMessageService
{
    List<IMessage> GetMessages<T>(string messageLocation) where T : IConvert;
    List<ICallRecord> GetCallRecords(string callLocation);
    List<ICustomerSubscription> GetCustomerRecords(string customerLocation);
}