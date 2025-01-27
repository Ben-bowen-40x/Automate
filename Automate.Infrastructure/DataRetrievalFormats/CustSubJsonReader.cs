using Automate.Translation.CustomerTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class CustSubJsonReader : ICustSubLongIdNumStrSellers
{
    public long SubscriptionId { get; set; }
    public long CustomerId { get; set; }
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset SubscriptionStartDate { get; set; }
    public string? Number1 { get; set; }
    public string? Number2 { get; set; }
    public DateTimeOffset CustomerCancelDate { get; set; }
    public DateTimeOffset SubscriptionCancelDate { get; set; }
    public int? CustomerActive { get; set; }
    public int? SubscriptionActive { get; set; }
    public int? InitialCompleted { get; set; }
    public double ContractValue { get; set; }
    public string? Seller1 { get; set; }
    public string? Seller2 { get; set; }
    public string? Seller3 { get; set; }
}