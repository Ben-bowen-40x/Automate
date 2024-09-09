using Microsoft.Extensions.DependencyInjection;
using Automate.Application;
using Automate.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Automate.Cli;

internal static class ConfigureCommandLine
{
   public static void ConfigureCli(this IServiceCollection services, IConfiguration configuration)
   {
      // Add settings and bind to configuration
      Settings settings = new();
      configuration.Bind(settings);
      typeof(Settings).GetInterfaces().ToList().ForEach(s => services.AddSingleton(s, settings));

      services.AddInfrastructure(settings).AddApplication();
   }
}
