namespace Automate.Infrastructure.DatabaseService;

public interface IDwhSettings : IRawQuerySettings
{
    public string? CallsConnectionString { get; set; }
    public string? CustomersConnectionString { get; set; }
    public string? ContactFormsConnectionString { get; set; }
}
