using Automate.Domain.ValueObjects;
using Automate.Translation.CustomerTranslate;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.QualifiedMessageTranslate;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.CustomerTranslate;

public static class CustomerSubscriptionTranslate
{
    /// <summary>
    /// Converts <see cref="ICustSubLongIdLongNumberStrSellers"/> into <see cref="ICustomerSubscription"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICustomerSubscription Translate(this ICustSubLongIdLongNumberStrSellers entity)
    {
        PhoneNumber number = PhoneNumberTranslate.Translate(entity.Number);

        // Translate sellers
        string sellers = VerifySeller(entity.Sellers);

        // Fix dates, which are in UTC already
        DateTime subStartDate = new(entity.SubStartDate.Ticks, DateTimeKind.Utc);
        DateTime custCxlDate = new(entity.CustomerCancelDate.Ticks, DateTimeKind.Utc);
        DateTime subCancelDate = new(entity.SubCancelDate.Ticks, DateTimeKind.Utc);

        // Retrieve customer info from the data
        ICustomerSubscription customer = new CustomerSubscription(entity.CustomerID, entity.SubId, entity.Date, new(subStartDate), number, PhoneNumberTranslate.Default, new(custCxlDate), new(subCancelDate), entity.SubIsActive, entity.SubIsActive, entity.CompletedInitial, entity.ContractValue, sellers);
        return customer;
    }

    /// <summary>
    /// Converts <see cref="ICustSubLongIdPhoneNumber"/> into <see cref="ICustomerSubscription"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICustomerSubscription Translate(this ICustSubLongIdPhoneNumber entity)
    {
        PhoneNumber number = entity.Number.Translate();
        PhoneNumber number2 = entity.Number2.Translate();
        string sellers = VerifySeller(entity.Sellers);
        return new CustomerSubscription(entity.CustomerId, entity.SubscriptionId, entity.Date, entity.SubscriptionStartDate, number, number2, entity.CustomerCancelDate, entity.SubscriptionCancelDate, entity.CustomerActive, entity.SubscriptionActive, entity.InitialCompleted, entity.DoubleValue, sellers);
    }

    /// <summary>
    /// Converts <see cref="ICustSubIntIdNumberStr"/> into <see cref="ICustomerSubscription"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICustomerSubscription Translate(this ICustSubIntIdNumberStr entity)
    {
        // Conversions
        long subId = entity.SubscriptionId;
        long customerId = entity.CustomerId;

        // ConvertTimeSpan StartDates
        DateTime dateInter = entity.Date is null ? DateTime.MinValue : (DateTime)entity.Date;
        DateTime subStartInter = entity.SubscriptionStartDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionStartDate;
        DateTimeOffset date = ConvertDateTimeOffset.ConvertLocalToDTOffset(dateInter, TimeZoneEnum.Pacific, out DateTimeOffset dateStartResult) ? dateStartResult : DateTimeOffset.MinValue;
        DateTimeOffset subDate = ConvertDateTimeOffset.ConvertLocalToDTOffset(subStartInter, TimeZoneEnum.Pacific, out DateTimeOffset subDateResult) ? subDateResult : DateTimeOffset.MinValue;

        // ConvertTimeSpan phone numbers
        PhoneNumber number1 = PhoneNumberTranslate.Translate(entity.Number1);
        PhoneNumber number2 = PhoneNumberTranslate.Translate(entity.Number2);

        // ConvertTimeSpan cancel date
        DateTime custCxlInter = entity.CustomerCancelDate is null ? DateTime.MinValue : (DateTime)entity.CustomerCancelDate;
        DateTime subCxlInter = entity.SubscriptionCancelDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionCancelDate;
        DateTimeOffset custCxl = ConvertDateTimeOffset.ConvertLocalToDTOffset(custCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset custResult) ? custResult : DateTimeOffset.MinValue;
        DateTimeOffset subCxl = ConvertDateTimeOffset.ConvertLocalToDTOffset(subCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset subResult) ? subResult : DateTimeOffset.MinValue;

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

    /// <summary>
    /// Converts <see cref="ICustSubLongIdNumStrSellers"/> into <see cref="ICustomerSubscription"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICustomerSubscription Translate(this ICustSubLongIdNumStrSellers entity)
    {
        // Translate dates
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.Date.DateTime, TimeZoneEnum.Pacific, DateTimeDefault.Max);
        DateTimeOffset subscriptionStartDate = ConvertPrimitive.ConvertDateTimeOffset(entity.SubscriptionStartDate.DateTime, TimeZoneEnum.Pacific, DateTimeDefault.Max);
        DateTimeOffset customerCancelDate = ConvertPrimitive.ConvertDateTimeOffset(entity.CustomerCancelDate.DateTime, TimeZoneEnum.Pacific, DateTimeDefault.Max);
        DateTimeOffset subscriptionCancelDate = ConvertPrimitive.ConvertDateTimeOffset(entity.SubscriptionCancelDate.DateTime, TimeZoneEnum.Pacific, DateTimeDefault.Max);

        // Translate Phone numbers
        PhoneNumber number1 = PhoneNumberTranslate.Translate(entity.Number1);
        PhoneNumber number2 = PhoneNumberTranslate.Translate(entity.Number2);

        // Translate booleans
        bool customerActive = ConvertPrimitive.ConvertBool(entity.CustomerActive);
        bool subscriptionActive = ConvertPrimitive.ConvertBool(entity.SubscriptionActive);
        bool initialCompleted = ConvertPrimitive.ConvertBool(entity.InitialCompleted);

        // Translate Sellers
        string sellers = VerifySeller(entity.Seller1, entity.Seller2, entity.Seller3);

        return new CustomerSubscription(entity.CustomerId, entity.SubscriptionId, date, subscriptionStartDate, number1, number2, customerCancelDate, subscriptionCancelDate, customerActive, subscriptionActive, initialCompleted, entity.ContractValue, sellers);
    }

    internal static string VerifySeller(params string?[] sellersArr)
    {
        string sellers;
        List<string> resultarr = new(sellersArr.Length);
        foreach (var seller in sellersArr)
            if (!string.IsNullOrWhiteSpace(seller)) resultarr.Add(seller);
        sellers = resultarr.Count > 0
            ? string.Join(" | ", sellersArr)
            : string.Empty;
        return sellers;
    }
}
