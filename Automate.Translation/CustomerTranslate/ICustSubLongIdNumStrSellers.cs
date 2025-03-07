namespace Automate.Translation.CustomerTranslate;

public interface ICustSubLongIdNumStrSellers
{
    long SubscriptionId { get; set; }
    long CustomerId { get; set; }
    DateTime Date { get; set; }
    DateTime SubscriptionStartDate { get; set; }
    string? Number1 { get; set; }
    string? Number2 { get; set; }
    DateTime CustomerCancelDate { get; set; }
    DateTime SubscriptionCancelDate { get; set; }
    int? CustomerActive { get; set; }
    int? SubscriptionActive { get; set; }
    int? InitialCompleted { get; set; }
    double ContractValue { get; set; }
    string? Seller1 { get; set; }
    string? Seller2 { get; set; }
    string? Seller3 { get; set; }
}
