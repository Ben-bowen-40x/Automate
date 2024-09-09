using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.JsonMaps;

public class CustSubJson : IDatedRecord
{
    public long SubscriptionId { get; set; }
    public long CustomerId { get; set; }
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset SubscriptionStartDate { get; set; }
    public NumberTypeJson? Number { get; set; }
    public NumberTypeJson? Number2 { get; set; }
    public DateTimeOffset CustomerCancelDate { get; set; }
    public DateTimeOffset SubscriptionCancelDate { get; set; }
    public bool CustomerActive { get; set; }
    public bool SubscriptionActive { get; set; }
    public bool InitialCompleted { get; set; }
    public double ContractValue { get; set; }
    public string? Sellers { get; set; }
    public CustomerSubscription Convert()
    {
        PhoneNumber number = Number is not null ? new(Number!.Number) : new(0);
        PhoneNumber number2 = Number2 is not null ? new(Number2!.Number) : new(0);
        string sellers = Sellers is not null ? Sellers! : string.Empty;
        return new CustomerSubscription(CustomerId, SubscriptionId, Date, SubscriptionStartDate, number, number2, CustomerCancelDate, SubscriptionCancelDate, CustomerActive, SubscriptionActive, InitialCompleted, ContractValue, sellers);
    }
}
