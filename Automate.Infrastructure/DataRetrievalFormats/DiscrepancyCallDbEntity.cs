using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Automate.Translation.DiscrepancyTranslate;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

[Keyless]
public class DiscrepancyCallDbEntity : IDiscrepancyEntity
{
    [Column("contact_number_clean")]
    public long Number { get; set; }
    [Column("called_at")]
    public DateTime? Date { get; set; }
    [Column("sale_billable")]
    public string? Billable { get; set; }
    [Column("duration")]
    public int? Duration { get; set; }
    [Column("note")]
    public string? Notes { get; set; }
}
