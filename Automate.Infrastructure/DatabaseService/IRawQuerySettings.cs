using Automate.Infrastructure.AngleService;

namespace Automate.Infrastructure.DatabaseService;

public interface IRawQuerySettings
{
    string? QueryDateFormat { get; set; }

    // Basics
    string? CallBasicNumerical { get; set; }
    string? CustomerBasicNumerical { get; set; }
    string? CallBasic { get; set; }
    string? CallBasicAddon { get; set; }
    string? CustomerBasic { get; set; }

    // Message
    string? MessageCallQuery1 { get; set; }
    string? MessageCallQuery2 { get; set; }
    string? MessageCallQuery3 { get; set; }
    string? MessageCustQuery2 { get; set; }
    string? MessageCustQuery3 { get; set; }

    // Discrepancy
    string? Discrepancy { get; set; }
    string? Discrepancy2 { get; set; }
    string? OriginalDiscrepancy { get; set; }

    // Contacts
    string? ContactUpdate1 { get; set; }
    string? ContactUpdate2 { get; set; }
    string? ContactUpdate3 { get; set; }
    ulong ContactUpdateNumber { get; set; }

    // Forms
    public string? WebFormQuery1 { get; set; }
    public string? WebFormQuery2 { get; set; }
}
