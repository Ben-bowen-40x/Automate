using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IMessageService
{
    List<IMessage> GetMessages<T>(FileInfo messageLocation) where T : IConvert;
    List<ICallRecord> GetCallRecords(IEnumerable<long> msgNums, FileInfo callLocation);
    List<ICustomerSubscription> GetCustomerRecords(IEnumerable<long> msgNums, FileInfo customerLocation);
}