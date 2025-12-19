using Microsoft.Extensions.DependencyInjection;
using Automate.Application;
using Automate.Infrastructure;
using Microsoft.Extensions.Configuration;
using Automate.Domain.SolutionFunctionality;

namespace Automate.Cli;

internal static class ConfigureCommandLine
{
    public static void ConfigureCli(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services
        services.AddScoped<IUserInformation, UserInformation>();

        // Add settings
        Settings settings = new();
        configuration.Bind(settings);

        // ConnectionStrings
        settings.CallsConnectionString =
            configuration.GetConnectionString("Calls");
        settings.CustomersConnectionString =
            configuration.GetConnectionString("Customers");
        settings.ContactFormsConnectionString =
            configuration.GetConnectionString("ContactForms");

        // register once
        services.AddSingleton(settings);

        // expose via interfaces
        typeof(Settings)
            .GetInterfaces()
            .ToList()
            .ForEach(i => services.AddSingleton(i, settings));

        services.AddInfrastructure(settings).AddApplication();
    }
}
