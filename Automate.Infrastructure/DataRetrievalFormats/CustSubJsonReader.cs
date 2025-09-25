using Automate.Translation.CustomerTranslate;
using Automate.Translation.ValueObjectTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class CustSubJsonReader : ICustSubLongIdNumStrSellers
{
    public long SubscriptionId { get; set; }
    public long CustomerId { get; set; }
    public DateTime Date { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public string? Number1 { get; set; }
    public string? Number2 { get; set; }
    public DateTime CustomerCancelDate { get; set; }
    public DateTime SubscriptionCancelDate { get; set; }
    public int? CustomerActive { get; set; }
    public int? SubscriptionActive { get; set; }
    public int? InitialCompleted { get; set; }
    public double ContractValue { get; set; }
    public string? Seller1 { get; set; }
    public string? Seller2 { get; set; }
    public string? Seller3 { get; set; }
    public TimeZoneEnum Zone => TimeZoneEnum.Pacific;
}
