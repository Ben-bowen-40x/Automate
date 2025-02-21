using Microsoft.Extensions.Configuration;

namespace Automate.Infrastructure.Test.TestConfigurations;

public class InfraTestConfiguration
{
    public InfraTestConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<IInfrastructureTestSettings>();
        IConfigurationRoot config = builder.Build();
        
        TestSettings = new InfrastructureTestSettings();
        config.Bind(TestSettings);
    }
    public IInfrastructureTestSettings TestSettings { get; }
}
