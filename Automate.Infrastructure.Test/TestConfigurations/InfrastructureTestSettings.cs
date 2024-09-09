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
    public string? LeafThreads { get; set; }
    

    // ICsvTestFileSettings
    public string? CsvAppendTestFile { get; set; }
}