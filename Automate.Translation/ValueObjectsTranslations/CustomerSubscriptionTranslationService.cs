using Automate.Domain.ValueObjects;
using Automate.Translation.DateTimeConvertService;
using Automate.Translation.MessageTranslationService;

namespace Automate.Translation.ValueObjectsTranslations;

public static class CustomerSubscriptionTranslationService
{
    // From ICustSub
    public static CustomerSubscription Convert(this ICustSub entity)
    { 
        // Conversions
        long subId = entity.SubscriptionId;
        long customerId = entity.CustomerId;

        // Convert StartDates
        DateTime dateInter = entity.Date is null ? DateTime.MinValue : (DateTime)entity.Date;
        DateTime subStartInter = entity.SubscriptionStartDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionStartDate;
        DateTimeOffset date = DateTimeConversions.ConvertLocalToDTOffset(dateInter, TimeZoneEnum.Pacific, out DateTimeOffset dateStartResult) ? dateStartResult : DateTimeOffset.MinValue;
        DateTimeOffset subDate = DateTimeConversions.ConvertLocalToDTOffset(subStartInter, TimeZoneEnum.Pacific, out DateTimeOffset subDateResult) ? subDateResult : DateTimeOffset.MinValue;

        // Convert phone numbers
        PhoneNumber number1 = entity.Number1 is null ? new(0) : new(entity.Number1);
        PhoneNumber number2 = entity.Number2 is null ? new(0) : new(entity.Number2);

        // Convert cancel date
        DateTime custCxlInter = entity.CustomerCancelDate is null ? DateTime.MinValue : (DateTime)entity.CustomerCancelDate;
        DateTime subCxlInter = entity.SubscriptionCancelDate is null ? DateTime.MinValue : (DateTime)entity.SubscriptionCancelDate;
        DateTimeOffset custCxl = DateTimeConversions.ConvertLocalToDTOffset(custCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset custResult) ? custResult : DateTimeOffset.MinValue;
        DateTimeOffset subCxl = DateTimeConversions.ConvertLocalToDTOffset(subCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset subResult) ? subResult : DateTimeOffset.MinValue;

        // Convert boolean States
        bool custActive = entity.CustomerActive is not null & entity.CustomerActive == 1;
        bool subActive = entity.SubscriptionActive is not null & entity.SubscriptionActive == 1;
        bool initial = entity.InitialCompleted is not null & entity.InitialCompleted == 1;

        // Convert Contract value
        double cv = entity.ContractValue is null ? 0.0 : (double)entity.ContractValue;

        // Gather sellers together
        List<string> sellersArr = new(3);
        if (entity.Seller1 is not null && entity.Seller1 != string.Empty) sellersArr.Add(entity.Seller1);
        if (entity.Seller2 is not null && entity.Seller2 != string.Empty) sellersArr.Add(entity.Seller2);
        if (entity.Seller3 is not null && entity.Seller3 != string.Empty) sellersArr.Add(entity.Seller3);
        string sellers = sellersArr.Count > 0 ? string.Join(" | ", sellersArr) : string.Empty;

        // Return result
        return new CustomerSubscription(customerId, subId, date, subDate, number1, number2, custCxl, subCxl, custActive, subActive, initial, cv, sellers);
    }
}
