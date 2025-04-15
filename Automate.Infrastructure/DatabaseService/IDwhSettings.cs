using Automate.Application.InfrastructureValueObjects;

namespace Automate.Infrastructure.DatabaseService;

public interface IDwhSettings : IRawQuerySettings
{
    string? CallsConnectionString { get; set; }
    string? CustomersConnectionString { get; set; }
    string? ContactFormsConnectionString { get; set; }
    string? GetConnectionString(DwhConnectionType type);
}
