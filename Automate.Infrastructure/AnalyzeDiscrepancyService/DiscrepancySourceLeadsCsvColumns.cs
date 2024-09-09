using CsvHelper.Configuration.Attributes;
using Automate.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Automate.Infrastructure.AnalyzeDiscrepancyService;

public partial class DiscrepancySourceLeadsCsvColumns : IComparable<DiscrepancySourceLeadsCsvColumns>
{
    // This class is used to retrieve WGL leads from a csv file
    // All leads from WGL in this way are billable because those are the only leads that are charged
    [Name("Caller Number")]
    public string? PhoneNumber { get; set; }
    [Name("Call Date & Time")]
    public string? StartDate { get; set; }
    [Name("Call Duration")]
    public string? Duration { get; set; }
    [Name("Notes")]
    public string? Notes { get; set; }
    public DiscrepancyCall Convert()
    {
        string rejoined = Notes is null
            ? string.Empty
            : string.Join("|", NewLineComma().Split(Notes));
        string notes = string.Join(string.Empty, DoubleQuotes().Split(rejoined));
        DateTime startDate = StartDate is null | !DateTime.TryParse(StartDate, out DateTime startResult) ? DateTime.MinValue : startResult;
        TimeSpan duration = Duration is null | !TimeSpan.TryParse(Duration, out TimeSpan durationResult) ? new(0) : durationResult;
        PhoneNumber number = PhoneNumber is null ? new(0) : new(PhoneNumber);

        return new(number, true, startDate, duration, notes); // Note that source leads are always billable
    }
    public int CompareTo(DiscrepancySourceLeadsCsvColumns? that)
    {
        if (this is null || StartDate is null || that is null) return -1;
        if (Convert().Date < that!.Convert().Date) return -1;
        if (Convert().Date == that!.Convert().Date) return 0;
        return 1;
    }

    [GeneratedRegex(@"\n|\r|\n\r|\r\n|,")]
    private static partial Regex NewLineComma();
    [GeneratedRegex("\"")]
    private static partial Regex DoubleQuotes();
}