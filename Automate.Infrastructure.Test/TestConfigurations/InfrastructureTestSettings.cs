using Automate.Application.InfrastructureValueObjects;
using Automate.Infrastructure.FatSap;

namespace Automate.Infrastructure.Test.TestConfigurations;

public class InfrastructureTestSettings : IInfrastructureTestSettings
{
    // ILeafTestSettings
    public string? LeafName { get; set; }
    public string? LeafTokenType { get; set; }
    public string? LeafRefreshToken { get; set; }
    public string? LeafBase { get; set; }
    public string? LeafAcctUuid { get; set; }
    public string? LeafUuid { get; set; }
    public string? LeafThreadsEndpoint { get; set; }
    public string? LeafMessagesEndpoint { get; set; }
    public string? LeafApiTestRepo { get; set; }

    // ICsvTestFileSettings
    public string? CsvAppendTestFile { get; set; }

    // ISharpQuerySettings
    public string? GuliagarName { get; set; }
    public string? GuliagarNameElement { get; set; }
    public string? GuliagarKey { get; set; }
    public string? GuliagarKeyElement { get; set; }
    public string? GuliagarBase { get; set; }
    public string? GuliagarUrl { get; set; }
    public string? GuliagarUrl2 { get; set; }
    public string? GuliagarBunny { get; set; }

    // IDwhTestSettings
    public string? CallsConnectionString { get; set; }
    public string? CustomersConnectionString { get; set; }
    public string? ContactFormsConnectionString { get; set; }
    public string? GetConnectionString(DwhConnectionType type) => type switch
    {
        DwhConnectionType.Calls => CallsConnectionString!,
        DwhConnectionType.Customers => CustomersConnectionString!,
        DwhConnectionType.ContactForms => ContactFormsConnectionString!,
        _ => throw new ArgumentException($"The given connection type has not been assigned a connection string:\n{type}")
    };
    public string? QueryDateFormat { get; set; }
    public string? CallBasicNumerical { get; set; }
    public string? CustomerBasicNumerical { get; set; }
    public string? CallBasic { get; set; }
    public string? CallBasicAddon { get; set; }
    public string? CustomerBasic { get; set; }
    public string? MessageCallQuery1 { get; set; }
    public string? MessageCallQuery2 { get; set; }
    public string? MessageCallQuery3 { get; set; }
    public string? MessageCustQuery1 { get; set; }
    public string? MessageCustQuery2 { get; set; }
    public string? CallsQuery { get; set; }
    public string? CallsQueryFilter { get; set; }
    public string? OriginalDiscrepancy { get; set; }
    public string? ContactUpdate1 { get; set; }
    public string? ContactUpdate2 { get; set; }
    public string? ContactUpdate3 { get; set; }
    public ulong ContactUpdateNumber { get; set; }
    public string? WebFormQuery { get; set; }
    public string? FatSapClientName { get; set; }
    public string? FatBaseEndpoint { get; set; }
    public string? FatAccountId { get; set; }
    public string? FatDateFormat { get; set; }
    public string? FatToken { get; set; }
}
