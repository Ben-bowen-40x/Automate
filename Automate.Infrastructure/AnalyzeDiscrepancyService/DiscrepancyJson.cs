using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.MessageLeadsService.JsonMaps;
using System.Text.RegularExpressions;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

internal class DiscrepancyJson
{
    public NumberTypeJson? Number { get; set; }
    public bool Billable { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
    internal DiscrepancyCall Convert()
    {
        PhoneNumber number = Number is null ? new(0) : new(Number.Number);
        string notes = Notes is not null ? string.Join(" | ", Regex.Split(Notes, @"\n|\r|\n\r|\r\n|,")) : string.Empty;
        return new DiscrepancyCall(number, Billable, Date, Duration, notes);
    }
}
