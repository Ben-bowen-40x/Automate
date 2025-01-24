using Automate.Domain.ValueObjects;
using Automate.Translation.DateTimeConvertService;
using Automate.Translation.InfrastructureInterfaces.Customer;

namespace Automate.Translation.ValueObjectsTranslations;

public static class CustomerSubscriptionTranslate
{
    // From ICustSubIntIdNumberStr
    public static CustomerSubscription Convert(this ICustSubIntIdNumberStr entity)
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
        PhoneNumber number1 = ConvertStringToPhoneNumber(entity.Number1);
        PhoneNumber number2 = ConvertStringToPhoneNumber(entity.Number2);

        // ConvertTimeSpan cancel date
        DateTime custCxlInter = entity.CustomerCancelDate is null ? DateTime.MinValue : (DateTime)entity.CustomerCancelDate;
        DateTime subCxlInter = entity.SubscriptionCancelDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionCancelDate;
        DateTimeOffset custCxl = DateTimeOffsetTranslate.ConvertLocalToDTOffset(custCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset custResult) ? custResult : DateTimeOffset.MinValue;
        DateTimeOffset subCxl = DateTimeOffsetTranslate.ConvertLocalToDTOffset(subCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset subResult) ? subResult : DateTimeOffset.MinValue;

        // ConvertTimeSpan boolean States
        bool custActive = ConvertIntToBool(entity.CustomerActive);
        bool subActive = ConvertIntToBool(entity.SubscriptionActive);
        bool initial = ConvertIntToBool(entity.InitialCompleted);

        // ConvertTimeSpan Contract value
        double cv = VerifyDouble(entity.ContractValue);

        // Gather sellers together
        string sellers = VerifySeller(entity.Seller1, entity.Seller2, entity.Seller3);

        // Return result
        return new CustomerSubscription(customerId, subId, date, subDate, number1, number2, custCxl, subCxl, custActive, subActive, initial, cv, sellers);
    }

    private static PhoneNumber ConvertStringToPhoneNumber(string? value)
    {
        return value is null ? new(0) : new(value);
    }

    private static bool ConvertIntToBool(int? value)
    {
        return value is not null && value > 0;
    }

    private static double VerifyDouble(double? value)
    {
        return value is null ? 0.0 : (double)value;
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

    public static CustomerSubscription Convert(this ICustSubLongIdPhoneNumber entity)
    {
        PhoneNumber number = entity.Number1 is not null ? entity.Number1.Convert() : new(0);
        PhoneNumber number2 = entity.Number2 is not null ? entity.Number2!.Convert() : new(0);
        string sellers = entity.Sellers is not null ? entity.Sellers! : string.Empty;
        return new CustomerSubscription(entity.CustomerId, entity.SubscriptionId, entity.Date, entity.SubscriptionStartDate, number, number2, entity.CustomerCancelDate, entity.SubscriptionCancelDate, entity.CustomerActive, entity.SubscriptionActive, entity.InitialCompleted, entity.ContractValue, sellers);
    }
}
