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
    public string? GuliagarUrl { get; set; }
    public string? GuliagarUrl2 { get; set; }
    public string? GuliagarBunny { get; set; }
}