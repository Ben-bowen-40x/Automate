using Automate.Domain;
using Automate.Infrastructure;

namespace Automate.Cli;

internal class Settings : IInfrastructureSettings, IDomainSettings
{
    #region IInfrastructureSettings
    
    // IDwhSettings
    public string? CallsConnectionString { get; set; }
    public string? CustomersConnectionString { get; set; }
    public string? ContactsConnectionString { get; set; }

    // ILeafApiSettings
    public string? LeafName { get; set; }
    public string? LeafTokenType { get; set; }
    public string? LeafRefreshToken { get; set; }
    public string? LeafBase { get; set; }
    public string? LeafAcctUuid { get; set; }
    public string? LeafUuid { get; set; }
    public string? LeafThreadsEndpoint { get; set; }

    #endregion

    #region IDomainSettings
    
    // IRawQuery settings
    public string? QueryDateFormat { get; set; }

    // Basics
    public string? CallBasicNumerical { get; set; }
    public string? CustomerBasicNumerical { get; set; }
    public string? CallBasic { get; set; }
    public string? CallBasicAddon { get; set; }
    public string? CustomerBasic { get; set; }

    // Message
    public string? MessageCallQuery1 { get; set; }
    public string? MessageCallQuery2 { get; set; }
    public string? MessageCallQuery3 { get; set; }
    public string? MessageCustQuery2 { get; set; }
    public string? MessageCustQuery3 { get; set; }

    // Discrepancy
    public string? Discrepancy { get; set; }
    public string? Discrepancy2 { get; set; }
    public string? OriginalDiscrepancy { get; set; }

    // Contacts
    public string? ContactUpdate1 { get; set; }
    public string? ContactUpdate2 { get; set; }
    public string? ContactUpdate3 { get; set; }
    public ulong ContactUpdateNumber { get; set; }

    // Forms
    public string? WebFormQuery1 { get; set; }
    public string? WebFormQuery2 { get; set; }

    // IMessagePatternSettings
    public string? Company { get; set; }
    public string? Name { get; set; }
    public string? CompanyType { get; set; }

    #endregion
}
