using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

[Keyless]
public partial class DiscrepancyCallDbEntity : IComparable<DiscrepancyCallDbEntity>
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
    public DiscrepancyCall Convert()
    {
        PhoneNumber number = new(Number);
        bool billable = Billable is not null && Billable != string.Empty && Billable == "billable";
        DateTime date = Date is not null ? (DateTime)Date! : DateTime.MinValue;
        string notes = Notes is not null ? string.Join("", DoubleQuotes().Split(string.Join(" | ", NewLineAndComma().Split(Notes)))) : string.Empty;
        TimeSpan duration = Duration is null ? new(0) : TimeSpan.FromSeconds((double)Duration!);

        return new(number, billable, date, duration, notes);
    }
    public int CompareTo(DiscrepancyCallDbEntity? that)
    {
        if (this is null || that is null || (Date < that!.Date)) return -1;
        if (Date == that!.Date) return 0;
        return 1;
    }

    [GeneratedRegex(@"\n|\r|\n\r|\r\n|,")]
    private static partial Regex NewLineAndComma();
    [GeneratedRegex("\"")]
    private static partial Regex DoubleQuotes();
}
