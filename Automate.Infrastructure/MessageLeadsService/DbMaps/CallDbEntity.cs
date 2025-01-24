using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Automate.Domain.ValueObjects;
using Automate.Translation.ValueObjectsTranslations;
using Automate.Translation.InfrastructureInterfaces.Message;

namespace Automate.Infrastructure.MessageLeadsService.DbMaps;

[Keyless]
public class CallDbEntity : IMsgZoneStr
{
    [Column("contact_number_clean")]
    public long NumberLong { get; set; }
    [Column("sale_billable")]
    public string? BillableStr { get; set; }
    [Column("called_at_utc")]
    public DateTime? Date { get; set; }
    [Column("time_zone")]
    public string? TimeZoneStr { get; set; }
    private PhoneNumber? _num;
    public PhoneNumber Number => _num ??= PhoneNumberTranslate.Convert(NumberLong);
}