using Microsoft.Extensions.Configuration;

namespace Automate.Infrastructure.Test.TestConfigurations;

public class InfrastructureConfiguration
{
    public InfrastructureConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<IInfrastructureTestSettings>();
        IConfigurationRoot config = builder.Build();
        
        Settings = new InfrastructureTestSettings();
        config.Bind(Settings);
    }
    public IInfrastructureTestSettings Settings { get; }
}
