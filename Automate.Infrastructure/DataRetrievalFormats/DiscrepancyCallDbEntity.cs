using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Automate.Translation.DiscrepancyTranslate;
using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

[Keyless]
public class DiscrepancyCallDbEntity : ICallBoolStringDateTime, IPhoneNumberCompatible
{
    [Column("contact_number_clean")]
    public long NumberLong { get; set; }
    [Column("called_at")]
    public DateTime? Date { get; set; }
    [Column("sale_billable")]
    public string? Billable { get; set; }
    [Column("duration")]
    public int? Duration { get; set; }
    [Column("note")]
    public string? Notes { get; set; }
    [Column("source")]
    public string? Source { get; set; }
    private PhoneNumber? _num;
    public PhoneNumber Number => _num ??= PhoneNumberTranslate.Translate(NumberLong);
}
