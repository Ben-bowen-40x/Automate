using Automate.Application.Discrepancy;
using Automate.Domain.SolutionFunctionality;
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
    [Fact(Skip = "This test is being deprecated")]
    public void DiscrepancyManager_ExecutesProperly()
    {
        // Log this instance as a test
        object sender = new DiscrepancyManager_Test();
        string memberName = nameof(DiscrepancyManager_Test);
        string location = GetFullName.GetMemberName(sender, memberName);
        StringLogger.NewLog(DateTime.Now, sender, memberName, $"Start Test");

        // Assemble
        DiscrepancyManager manager = new(new DiscrepancyService(_settings), new ReportServiceSingleton());

        // Act
        Result<FileInfo> result = manager.ManageDiscrepancyAnalysis("", "", "");
        StringLogger.ProduceLog(DateTime.Now, sender, memberName, $"End Test");

        // Assert
        Assert.True(result.IsSuccess);
    }
}