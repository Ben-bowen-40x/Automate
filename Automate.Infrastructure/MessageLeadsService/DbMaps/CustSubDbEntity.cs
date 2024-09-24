using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DateTimeConversion;
using Automate.Application.InfrastructureInterfaces;

namespace Automate.Infrastructure.MessageLeadsService.DbMaps;

[PrimaryKey("SubscriptionId")]
public class CustSubDbEntity : IPhoneNumberCompatible
{
    [Column("SubscriptionId")]
    public required int SubscriptionId { get; set; }
    [Column("CustomerId")]
    public required int CustomerId { get; set; }
    [Column("Date")]
    public DateTime? Date { get; set; }
    [Column("SubscriptionStartDate")]
    public DateTime? SubscriptionStartDate { get; set; }
    [Column("Number")]
    public string? Number1 { get; set; }
    [Column("Number2")]
    public string? Number2 { get; set; }
    [Column("CustomerCancelDate")]
    public DateTime? CustomerCancelDate { get; set; }
    [Column("SubscriptionCancelDate")]
    public DateTime? SubscriptionCancelDate { get; set; }
    [Column("CustomerActive")]
    public int? CustomerActive { get; set; }
    [Column("SubscriptionActive")]
    public int? SubscriptionActive { get; set; }
    [Column("InitialCompleted")]
    public int? InitialCompleted { get; set; }
    [Column("ContractValue")]
    public double? ContractValue { get; set; }
    [Column("Seller1")]
    public string? Seller1 { get; set; }
    [Column("Seller2")]
    public string? Seller2 { get; set; }
    [Column("Seller3")]
    public string? Seller3 { get; set; }

    private PhoneNumber? num;
    public PhoneNumber Number => num ??= Number1 is null ? new(0) : new(Number1);

    public CustomerSubscription Convert()
    {
        // Conversions
        long subId = SubscriptionId;
        long customerId = CustomerId;

        // Convert StartDates
        DateTime dateInter = Date is null ? DateTime.MinValue : (DateTime)Date;
        DateTime subStartInter = SubscriptionStartDate is null ? DateTime.MinValue : (DateTime)SubscriptionStartDate;
        DateTimeOffset date = DateTimeConversions.ConvertLocalToDTOffset(dateInter, TimeZoneEnum.Pacific, out DateTimeOffset dateStartResult) ? dateStartResult : DateTimeOffset.MinValue;
        DateTimeOffset subDate = DateTimeConversions.ConvertLocalToDTOffset(subStartInter, TimeZoneEnum.Pacific, out DateTimeOffset subDateResult) ? subDateResult : DateTimeOffset.MinValue;

        // Convert phone numbers
        PhoneNumber number1 = Number1 is null ? new(0) : new(Number1);
        PhoneNumber number2 = Number2 is null ? new(0) : new(Number2);

        // Convert cancel date
        DateTime custCxlInter = CustomerCancelDate is null ? DateTime.MinValue : (DateTime)CustomerCancelDate;
        DateTime subCxlInter = SubscriptionCancelDate is null ? DateTime.MinValue : (DateTime)SubscriptionCancelDate;
        DateTimeOffset custCxl = DateTimeConversions.ConvertLocalToDTOffset(custCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset custResult) ? custResult : DateTimeOffset.MinValue;
        DateTimeOffset subCxl = DateTimeConversions.ConvertLocalToDTOffset(subCxlInter, TimeZoneEnum.Pacific, out DateTimeOffset subResult) ? subResult : DateTimeOffset.MinValue;

        // Convert boolean States
        bool custActive = CustomerActive is not null & CustomerActive == 1;
        bool subActive = SubscriptionActive is not null & SubscriptionActive == 1;
        bool initial = InitialCompleted is not null & InitialCompleted == 1;

        // Convert Contract value
        double cv = ContractValue is null ? 0.0 : (double)ContractValue;

        // Gather sellers together
        List<string> sellersArr = new(3);
        if (Seller1 is not null && Seller1 != string.Empty) sellersArr.Add(Seller1);
        if (Seller2 is not null && Seller2 != string.Empty) sellersArr.Add(Seller2);
        if (Seller3 is not null && Seller3 != string.Empty) sellersArr.Add(Seller3);
        string sellers = sellersArr.Count > 0 ? string.Join(" | ", sellersArr) : string.Empty;

        // Return result
        return new CustomerSubscription(customerId, subId, date, subDate, number1, number2, custCxl, subCxl, custActive, subActive, initial, cv, sellers);
    }
}
