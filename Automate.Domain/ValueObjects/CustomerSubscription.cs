namespace Automate.Domain.ValueObjects;

public record CustomerSubscription(long CustomerId, long SubscriptionId, DateTimeOffset Date, DateTimeOffset SubscriptionStartDate, PhoneNumber Number, PhoneNumber Number2, DateTimeOffset CustomerCancelDate, DateTimeOffset SubscriptionCancelDate, bool CustomerActive, bool SubscriptionActive, bool InitialCompleted, double ContractValue, string Sellers) : ICustomerSubscription
{
    public DateTimeOffset Date { get; set; } = Date;
    public override string ToString()
    {
        return $"{nameof(CustomerSubscription)}: {nameof(CustomerId)}: {CustomerId}, {nameof(SubscriptionId)}: {SubscriptionId}, Customer start date: {Date.ToString(DateTimeStrings.InternalDateTimeOffset)}, {nameof(SubscriptionStartDate)}: {SubscriptionStartDate.ToString(DateTimeStrings.InternalDateTimeOffset)}, {nameof(Number)}: {Number.Number}, {nameof(Number2)}: {Number2.Number}, {nameof(CustomerCancelDate)}: {CustomerCancelDate.ToString(DateTimeStrings.InternalDateTimeOffset)}, {nameof(SubscriptionCancelDate)}: {SubscriptionCancelDate.ToString(DateTimeStrings.InternalDateTimeOffset)}, {nameof(CustomerActive)}: {CustomerActive}, {nameof(SubscriptionActive)}: {SubscriptionActive}, {nameof(InitialCompleted)}: {InitialCompleted}, {nameof(ContractValue)}: {ContractValue}";
    }
}
