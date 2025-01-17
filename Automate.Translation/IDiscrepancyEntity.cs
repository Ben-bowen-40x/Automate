using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace Automate.Translation;

public interface IDiscrepancyEntity
{
    public long Number { get; set; }
    public DateTime? Date { get; set; }
    public string? Billable { get; set; }
    public int? Duration { get; set; }
    public string? Notes { get; set; }
}
