using Microsoft.Extensions.DependencyInjection;
using Automate.Application.InfrastructureInterfaces;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.ReportingService;
using Automate.Infrastructure.MessageLeadsService;
using Automate.Infrastructure.ContactsUpdateService;
using Automate.Infrastructure.JsonToCsvService;
using Automate.Infrastructure.MessageLeadsReportService;
using Automate.Infrastructure.LeafClientService;
using Automate.Infrastructure.DwhRepoUpdateService;
using System.Net;

namespace Automate.Infrastructure;

public static class InjectInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IInfrastructureSettings settings)
    {
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IReportMessageService, ReportMessageService>();
        services.AddScoped<IUpdateContactsService, UpdateContactsService>();
        services.AddScoped<IDiscrepancyService, DiscrepancyService>();
        services.AddScoped<IReportService, ReportServiceSingleton>();
        services.AddScoped<IJsonConversionService, JsonConversionService>();
        services.AddScoped<ILeafApiService, LeafApiService>();
        services.AddScoped<IDwhRepoUpdateService, DwhRepoService>();

        services.AddHttpClient();

        // Add Leaf Client
        services.AddHttpClient(settings.LeafName!, c =>
        {
            c.BaseAddress = new Uri(settings.LeafBase!);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.DefaultRequestHeaders.Add("Authorization", settings.LeafTokenType);
        });

        // Add client with cookies
        services.AddHttpClient(settings.Cookie!)
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    CookieContainer = new CookieContainer(),
                    UseCookies = true, // Ensure that the handler uses the CookieContainer
                };
            });

        // Add client without cookies
        services.AddHttpClient(settings.NoCookie!)
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    UseCookies = false,
                };
            });

        return services;
    }
}
