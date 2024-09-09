namespace Automate.Domain.ValueObjects;

public interface ICustomerSubscription : IDatedRecord
{
    long CustomerId { get; init; }
    long SubscriptionId { get; init; }
    DateTimeOffset SubscriptionStartDate { get; init; }
    PhoneNumber Number { get; init; }
    PhoneNumber Number2 { get; init; }
    DateTimeOffset CustomerCancelDate { get; init; }
    DateTimeOffset SubscriptionCancelDate { get; init; }
    bool CustomerActive { get; init; }
    bool SubscriptionActive { get; init; }
    bool InitialCompleted { get; init; }
    double ContractValue { get; init; }
    string Sellers { get; init; }
    string ToString();

}