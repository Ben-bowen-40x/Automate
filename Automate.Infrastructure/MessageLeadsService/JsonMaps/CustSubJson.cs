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
public class CustSubJsonReader : IDatedRecord
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
    public CustomerSubscription Convert()
    {
        // Convert Phone numbers
        PhoneNumber number1 = Number1 is not null && long.TryParse(Number1, out long num1) ? new(num1) : new(0);
        PhoneNumber number2 = Number2 is not null && long.TryParse(Number2, out long num2) ? new(num2) : new(0);

        // Convert booleans
        bool customerActive = CustomerActive is not null && CustomerActive != 0;
        bool subscriptionActive = SubscriptionActive is not null && SubscriptionActive != 0;
        bool initialCompleted = InitialCompleted is not null && InitialCompleted != 0;

        // Convert Sellers
        List<string> sellersArr = new(3);
        if (Seller1 is not null && Seller1 != string.Empty) sellersArr.Add(Seller1);
        if (Seller2 is not null && Seller2 != string.Empty) sellersArr.Add(Seller2);
        if (Seller3 is not null && Seller3 != string.Empty) sellersArr.Add(Seller3);
        string sellers = sellersArr.Count > 0 ? string.Join(" | ", sellersArr) : string.Empty;


        return new CustomerSubscription(CustomerId, SubscriptionId, Date, SubscriptionStartDate, number1, number2, CustomerCancelDate, SubscriptionCancelDate, customerActive, subscriptionActive, initialCompleted, ContractValue, sellers);
    }
}