using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Infrastructure.Test.TestConfigurations;

public class InfraTestConfiguration
{
    public InfraTestConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<IInfrastructureTestSettings>();
        IConfigurationRoot config = builder.Build();

        // Set up DI container
        var services = new ServiceCollection();
        services.AddScoped<IInfrastructureTestSettings, InfrastructureTestSettings>();
        services.AddHttpClient(); // Register IHttpsClientFactory

        // Build ServiceProvider
        var provider = services.BuildServiceProvider();
        ClientFactory = provider.GetRequiredService<IHttpClientFactory>(); // Resolve IHttpClientFactory

        // Bind configuration to TestSettings
        InfrastructureTestSettings settings = new();
        typeof(InfrastructureTestSettings).GetInterfaces().ToList().ForEach(s => services.AddSingleton(s, settings));
        config.Bind(settings);
        TestSettings = settings;

        // Add FatSap client
        services.AddHttpClient(settings.FatSapClientName!, c =>
        {
            c.BaseAddress = new Uri(settings.FatBaseEndpoint!);
            c.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", settings.FatToken!);
        });
    }

    public IInfrastructureTestSettings TestSettings { get; }
    public IHttpClientFactory ClientFactory { get; }
}
