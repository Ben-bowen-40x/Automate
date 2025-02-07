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
    public string? QueryDateFormat { get; set; }
    public string? CallBasicNumerical { get; set; }
    public string? CustomerBasicNumerical { get; set; }
    public string? CallBasic { get; set; }
    public string? CallBasicAddon { get; set; }
    public string? CustomerBasic { get; set; }
    public string? MessageCallQuery1 { get; set; }
    public string? MessageCallQuery2 { get; set; }
    public string? MessageCallQuery3 { get; set; }
    public string? MessageCustQuery2 { get; set; }
    public string? MessageCustQuery3 { get; set; }
    public string? Discrepancy { get; set; }
    public string? Discrepancy2 { get; set; }
    public string? OriginalDiscrepancy { get; set; }
    public string? ContactUpdate1 { get; set; }
    public string? ContactUpdate2 { get; set; }
    public string? ContactUpdate3 { get; set; }
    public ulong ContactUpdateNumber { get; set; }
    public string? WebFormQuery1 { get; set; }
    public string? WebFormQuery2 { get; set; }
}