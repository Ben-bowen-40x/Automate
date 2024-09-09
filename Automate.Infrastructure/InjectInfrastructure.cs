using Microsoft.Extensions.DependencyInjection;
using Automate.Application.InfrastructureInterfaces;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.ReportingService;
using Automate.Infrastructure.MessageLeadsService;
using Automate.Infrastructure.ContactsUpdateService;
using Automate.Infrastructure.JsonToCsvService;
using Automate.Infrastructure.MessageLeadsReportService;

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
        services.AddHttpClient();

        // Add Leaf Client
        services.AddHttpClient(settings.LeafName!, c =>
        {
            c.BaseAddress = new Uri(settings.LeafBase!);
            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.DefaultRequestHeaders.Add("Authorization", settings.LeafTokenType);
        });
        return services;
    }
}
