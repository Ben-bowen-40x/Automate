using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Automate.Domain.ValueObjects;
using Automate.Translation.CallTranslate;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

[Keyless]
public class CallDbEntity : ICallDateTimeInUTC
{
    [Column("contact_number_clean")]
    public long NumberLong { get; set; }
    [Column("sale_billable")]
    public string? Billable { get; set; }
    [Column("called_at_utc")]
    public DateTime? Date { get; set; }
    [Column("time_zone")]
    public string? TimeZone { get; set; }
    private PhoneNumber? _num;
    public PhoneNumber Number => _num ??= PhoneNumberTranslate.Translate(NumberLong);
}