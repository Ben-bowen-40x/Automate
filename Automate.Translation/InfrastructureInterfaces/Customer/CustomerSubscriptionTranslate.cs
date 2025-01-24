using Automate.Domain.ValueObjects;
using Automate.Translation.DateTimeConvertService;
using Automate.Translation.InfrastructureInterfaces.Customer;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.InfrastructureInterfaces.Customer;

public static class CustomerSubscriptionTranslate
{
    /// <summary>
    /// Converts <see cref="ICustSubLongIdPhoneNumber"/> into <see cref="ICustomerSubscription"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICustomerSubscription Convert(this ICustSubLongIdPhoneNumber entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Convert(entity.Number);
        PhoneNumber number2 = PhoneNumberTranslate.Convert(entity.Number2);
        string sellers = VerifySeller(entity.Sellers);
        return new CustomerSubscription(entity.CustomerId, entity.SubscriptionId, entity.Date, entity.SubscriptionStartDate, number, number2, entity.CustomerCancelDate, entity.SubscriptionCancelDate, entity.CustomerActive, entity.SubscriptionActive, entity.InitialCompleted, entity.DoubleValue, sellers);
    }

    /// <summary>
    /// Converts <see cref="ICustSubIntIdNumberStr"/> to <see cref="ICustomerSubscription"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICustomerSubscription Convert(this ICustSubIntIdNumberStr entity)
    {
        // Conversions
        long subId = entity.SubscriptionId;
        long customerId = entity.CustomerId;

        // ConvertTimeSpan StartDates
        DateTime dateInter = entity.Date is null ? DateTime.MinValue : (DateTime)entity.Date;
        DateTime subStartInter = entity.SubscriptionStartDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionStartDate;
        DateTimeOffset date = DateTimeOffsetTranslate.ConvertLocalToDTOffset(dateInter, TimeZoneEnum.Pacific, out DateTimeOffset dateStartResult) ? dateStartResult : DateTimeOffset.MinValue;
        DateTimeOffset subDate = DateTimeOffsetTranslate.ConvertLocalToDTOffset(subStartInter, TimeZoneEnum.Pacific, out DateTimeOffset subDateResult) ? subDateResult : DateTimeOffset.MinValue;

        // ConvertTimeSpan phone numbers
        PhoneNumber number1 = PhoneNumberTranslate.Convert(entity.Number1);
        PhoneNumber number2 = PhoneNumberTranslate.Convert(entity.Number2);

        // ConvertTimeSpan cancel date
        DateTime custCxlInter = entity.CustomerCancelDate is null ? DateTime.MinValue : (DateTime)entity.CustomerCancelDate;
        DateTime subCxlInter = entity.SubscriptionCancelDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionCancelDate;
        DateTimeOffset custCxl = DateTimeOffsetTranslate.ConvertLocalToDTOffset(custCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset custResult) ? custResult : DateTimeOffset.MinValue;
        DateTimeOffset subCxl = DateTimeOffsetTranslate.ConvertLocalToDTOffset(subCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset subResult) ? subResult : DateTimeOffset.MinValue;

        // ConvertTimeSpan boolean States
        bool custActive = ConvertPrimitive.ConvertBool(entity.CustomerActive);
        bool subActive = ConvertPrimitive.ConvertBool(entity.SubscriptionActive);
        bool initial = ConvertPrimitive.ConvertBool(entity.InitialCompleted);

        // ConvertTimeSpan Contract value
        double cv = ConvertPrimitive.VerifyValue(entity.DoubleValue);

        // Gather sellers together
        string sellers = VerifySeller(entity.Seller1, entity.Seller2, entity.Seller3);

        // Return result
        return new CustomerSubscription(customerId, subId, date, subDate, number1, number2, custCxl, subCxl, custActive, subActive, initial, cv, sellers);
    }

    public static CustomerSubscription Convert(this ICustSubLongIdNumStrSellers entity)
    {
        // Convert dates
        var date = DateTimeOffsetTranslate.ConvertLocalToDTOffset(entity.Date.DateTime, TimeZoneEnum.Pacific, out DateTimeOffset dateResult)
            ? dateResult
            : DateTimeOffset.MaxValue;
        var subscriptionStartDate = DateTimeOffsetTranslate.ConvertLocalToDTOffset(entity.SubscriptionStartDate.DateTime, TimeZoneEnum.Pacific, out DateTimeOffset subResult)
            ? subResult
            : DateTimeOffset.MaxValue;
        var customerCancelDate = DateTimeOffsetTranslate.ConvertLocalToDTOffset(entity.CustomerCancelDate.DateTime, TimeZoneEnum.Pacific, out DateTimeOffset cxlResult)
            ? cxlResult
            : DateTimeOffset.MaxValue;
        var subscriptionCancelDate = DateTimeOffsetTranslate.ConvertLocalToDTOffset(entity.SubscriptionCancelDate.DateTime, TimeZoneEnum.Pacific, out DateTimeOffset sxlResult)
            ? sxlResult
            : DateTimeOffset.MaxValue;

        // Convert Phone numbers
        PhoneNumber number1 = entity.Number1 is not null && long.TryParse(entity.Number1, out long num1) ? new(num1) : new(0);
        PhoneNumber number2 = entity.Number2 is not null && long.TryParse(entity.Number2, out long num2) ? new(num2) : new(0);

        // Convert booleans
        bool customerActive = entity.CustomerActive is not null && entity.CustomerActive != 0;
        bool subscriptionActive = entity.SubscriptionActive is not null && entity.SubscriptionActive != 0;
        bool initialCompleted = entity.InitialCompleted is not null && entity.InitialCompleted != 0;

        // Convert Sellers
        List<string> sellersArr = new(3);
        if (entity.Seller1 is not null && entity.Seller1 != string.Empty) sellersArr.Add(entity.Seller1);
        if (entity.Seller2 is not null && entity.Seller2 != string.Empty) sellersArr.Add(entity.Seller2);
        if (entity.Seller3 is not null && entity.Seller3 != string.Empty) sellersArr.Add(entity.Seller3);
        string sellers = sellersArr.Count > 0 ? string.Join(" | ", sellersArr) : string.Empty;


        return new CustomerSubscription(entity.CustomerId, entity.SubscriptionId, date, subscriptionStartDate, number1, number2, customerCancelDate, subscriptionCancelDate, customerActive, subscriptionActive, initialCompleted, entity.ContractValue, sellers);
    }

    private static string VerifySeller(params string?[] sellersArr)
    {
        string sellers;
        List<string> resultarr = [];
        foreach (var seller in sellersArr)
            if (seller is not null && seller != string.Empty) resultarr.Add(seller);
        sellers = resultarr.Count > 0 ? string.Join(" | ", sellersArr) : string.Empty;
        return sellers;
    }

}
