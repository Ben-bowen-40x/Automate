namespace Automate.Domain.MessageAnalysis.MessageSettings;

public interface IMessagePatternSettings
{
    public string? Company { get; set; }
    public string? Name { get; set; }
    public string? CompanyType { get; set; }
}
