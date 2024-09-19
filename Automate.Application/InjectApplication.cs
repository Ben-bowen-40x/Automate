using Microsoft.Extensions.DependencyInjection;
using Automate.Application.Discrepancy;
using Automate.Application.MessageAnalysis;
using Automate.Application.UpdateContacts;
using Automate.Application.JsonCsvConversion;
using Automate.Application.MessageReportAnalysis;
using Automate.Application.ApiRepoUpdate;

namespace Automate.Application;

public static class InjectApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMessageAnalysisManager, MessageAnalysisManager>();
        services.AddScoped<IMessageAnalysisReportManager, MessageAnalysisReportManager>();
        services.AddScoped<IContactUpdateManager, ContactUpdateManager>();
        services.AddScoped<IDiscrepancyManager, DiscrepancyManager>();
        services.AddScoped<IJsonToCsvConversionManager, JsonToCsvConversionManager>();
        services.AddScoped<IRepoUpdateManager, LeafApiRepoUpdateManager>();
        return services;
    }
}
