using Automate.Domain;
using Automate.Infrastructure;

namespace Automate.Cli;

internal class Settings : IInfrastructureSettings, IDomainSettings
{
    // IDwhSettings
    public string? CallsConnectionString { get; set; }
    public string? CustomersConnectionString { get; set; }

    // ILeafApiSettings
    public string? LeafName { get; set; }
    public string? LeafTokenType { get; set; }
    public string? LeafRefreshToken { get; set; }
    public string? LeafBase { get; set; }
    public string? LeafAcctUuid { get; set; }
    public string? LeafUuid { get; set; }
    public string? LeafThreadsEndpoint { get; set; }

    // IMessagePatternSettings
    public string? Company { get; set; }
    public string? Name { get; set; }
    public string? CompanyType { get; set; }
}
