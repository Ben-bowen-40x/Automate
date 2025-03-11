using Automate.Domain;
using Automate.Infrastructure;

namespace Automate.Cli;

internal class Settings : IInfrastructureSettings, IDomainSettings
{
    #region IInfrastructureSettings

    // IInfrastructureSettings
    public string? Cookie { get; set; } = "cookie";
    public string? NoCookie { get; set; } = "no-cookie";

    // IDwhSettings
    public string? CallsConnectionString { get; set; }
    public string? CustomersConnectionString { get; set; }
    public string? ContactFormsConnectionString { get; set; }

    // IRawQuery settings
    public string? QueryDateFormat { get; set; }

    // Basics
    public string? CallBasic { get; set; }
    public string? CallBasicNumerical { get; set; }
    public string? CustomerBasicNumerical { get; set; }
    public string? CallBasicAddon { get; set; }
    public string? CustomerBasic { get; set; }

    // Message
    public string? MessageCallQuery1 { get; set; }
    public string? MessageCallQuery2 { get; set; }
    public string? MessageCallQuery3 { get; set; }
    public string? MessageCustQuery1 { get; set; }
    public string? MessageCustQuery2 { get; set; }

    // ILeafApiSettings
    public string? LeafName { get; set; }
    public string? LeafTokenType { get; set; }
    public string? LeafRefreshToken { get; set; }
    public string? LeafBase { get; set; }
    public string? LeafAcctUuid { get; set; }
    public string? LeafUuid { get; set; }
    public string? LeafThreadsEndpoint { get; set; }

    // Discrepancy
    public string? CallsQuery { get; set; }
    public string? CallsQueryFilter { get; set; }
    public string? OriginalDiscrepancy { get; set; }

    // Contacts
    public string? ContactUpdate1 { get; set; }
    public string? ContactUpdate2 { get; set; }
    public string? ContactUpdate3 { get; set; }
    public ulong ContactUpdateNumber { get; set; }

    // Forms
    public string? WebFormQuery { get; set; }

    // ISharpQuerySettings
    public string? GuliagarName { get; set; }
    public string? GuliagarNameElement { get; set; }
    public string? GuliagarKey { get; set; }
    public string? GuliagarKeyElement { get; set; }
    public string? GuliagarBase { get; set; }
    public string? GuliagarUrl { get; set; }
    public string? GuliagarUrl2 { get; set; }
    public string? GuliagarBunny { get; set; }

    #endregion

    #region IDomainSettings
    // IMessagePatternSettings
    public string? Company { get; set; }
    public string? Name { get; set; }
    public string? CompanyType { get; set; }

    #endregion
}
