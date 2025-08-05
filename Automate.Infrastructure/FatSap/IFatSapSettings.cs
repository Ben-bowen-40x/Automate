namespace Automate.Infrastructure.FatSap;

public interface IFatSapSettings
{
    public string? FatSapClientName { get; set; }
    public string? FatBaseEndpoint { get; set; }
    public string? FatAccountId { get; set; }
    public string? FatDateFormat { get; set; }
    public string? FatToken { get; set; }
}