using Automate.Application.Discrepancy;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.ReportingService;
using Automate.Infrastructure.Test.TestConfigurations;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.Test.DiscrepancyTest;

public class DiscrepancyManager_Test
{
    public DiscrepancyManager_Test()
    {
        _settings = new InfraTestConfiguration().TestSettings;
    }
    private readonly IDwhTestSettings _settings;
    [
        Fact
        (Skip = "This test is being deprecated")
    ]
    public void DiscrepancyManager_ExecutesProperly()
    {
        // Assemble
        DiscrepancyManager manager = new(new DiscrepancyService(_settings), new ReportServiceSingleton());

        // Act
        Result<FileInfo> result = manager.ManageDiscrepancyAnalysis("", "", "");

        // Assert
        Assert.True(result.IsSuccess);
    }
}