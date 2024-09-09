namespace Automate.Infrastructure.DatabaseService;

public interface IDwhSettings
{
    public string? CallsConnectionString { get; set; }
    public string? CustomersConnectionString { get; set; }
}
