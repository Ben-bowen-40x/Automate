using Automate.Domain.ValueObjects;
using Automate.Translation.CustomerTranslate;
using Automate.Translation.PhoneNumTranslate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Automate.Infrastructure.DataRetrievalFormats;

[PrimaryKey("SubscriptionId")]
public class CustSubDbEntity : ICustSubIntIdNumberStr
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
    public PhoneNumber Number => num ??= PhoneNumberTranslate.Translate(Number1);
}
