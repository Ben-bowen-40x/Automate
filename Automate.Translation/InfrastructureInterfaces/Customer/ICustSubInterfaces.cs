using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.InfrastructureInterfaces.Customer;

public interface ICustSubLongIdPhoneNumber : ICustSub_CustSubLong, ICustSub_NumberType, ICustSub_DateTimeOffset, ICustSub_Bools, ICustSub_CVDouble, ICustSub_SellersStr { }
public interface ICustSubIntIdNumberStr : IPhoneNumberCompatible, ICustSub_CustSubInt, ICustSub_NumberStr, ICustSub_NullableDateTime, ICustSub_BoolAsInt, ICustSub_Seller3Str, ICustSub_CVNullableDouble { }
public interface ICustSubLongIdNumStrSellers : ICustSub_CustSubLong, ICustSub_DateTimeOffset, ICustSub_NumberStr, ICustSub_BoolAsInt, ICustSub_Seller3Str { }

public interface ICustSub_CustSubInt
{
    int SubscriptionId { get; set; }
    int CustomerId { get; set; }
}
public interface ICustSub_CustSubLong
{
    long SubscriptionId { get; set; }
    long CustomerId { get; set; }
}
public interface ICustSub_NumberStr
{
    string? Number1 { get; set; }
    string? Number2 { get; set; }
}
public interface ICustSub_NumberType
{
    IPhoneNumberTranslate? Number1 { get; set; }
    IPhoneNumberTranslate? Number2 { get; set; }
}
public interface ICustSub_NullableDateTime
{
    DateTime? Date { get; set; }
    DateTime? SubscriptionStartDate { get; set; }
    DateTime? CustomerCancelDate { get; set; }
    DateTime? SubscriptionCancelDate { get; set; }
}
public interface ICustSub_DateTimeOffset : IDatedRecord
{
    DateTimeOffset SubscriptionStartDate { get; set; }
    DateTimeOffset CustomerCancelDate { get; set; }
    DateTimeOffset SubscriptionCancelDate { get; set; }
}
public interface ICustSub_BoolAsInt
{
    int? CustomerActive { get; set; }
    int? SubscriptionActive { get; set; }
    int? InitialCompleted { get; set; }
}
public interface ICustSub_Bools
{
    bool CustomerActive { get; set; }
    bool SubscriptionActive { get; set; }
    bool InitialCompleted { get; set; }
}
public interface ICustSub_Seller3Str
{
    string? Seller1 { get; set; }
    string? Seller2 { get; set; }
    string? Seller3 { get; set; }
}
public interface ICustSub_SellersStr
{
    string? Sellers { get; set; }
}
public interface ICustSub_CVNullableDouble
{
    double? ContractValue { get; set; }
}
public interface ICustSub_CVDouble
{
    double ContractValue { get; set; }
}
