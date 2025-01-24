using Automate.Domain.ValueObjects;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.InfrastructureInterfaces.Customer;

public interface ICustSubLongIdPhoneNumber : IDatedRecord
{
    public long SubscriptionId { get; set; }
    public long CustomerId { get; set; }
    public DateTimeOffset SubscriptionStartDate { get; set; }
    public IPhoneNumberTranslate? Number { get; set; }
    public IPhoneNumberTranslate? Number2 { get; set; }
    public DateTimeOffset CustomerCancelDate { get; set; }
    public DateTimeOffset SubscriptionCancelDate { get; set; }
    public bool CustomerActive { get; set; }
    public bool SubscriptionActive { get; set; }
    public bool InitialCompleted { get; set; }
    public double DoubleValue { get; set; }
    public string? Sellers { get; set; }
}

