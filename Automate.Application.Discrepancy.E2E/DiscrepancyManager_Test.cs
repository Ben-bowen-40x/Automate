using Automate.Application.Discrepancy;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.ReportingService;
using CSharpFunctionalExtensions;

namespace Automate.Application.EndToEndTest.DiscrepancyManagement;

public class DiscrepancyManager_Test(IDwhSettings settings)
{
    [Fact]
    public void DiscrepancyManager_ExecutesProperly()
    {
        // Log this instance as a test
        object sender = new DiscrepancyManager_Test(settings);
        string memberName = nameof(DiscrepancyManager_Test);
        string location = GetFullName.GetMemberName(sender, memberName);
        StringLogger.NewLog(DateTime.Now, sender, memberName, $"Start Test");

        // Assemble
        DiscrepancyManager manager = new(new DiscrepancyService(settings), new ReportServiceSingleton());

        // Act
        Result<FileInfo> result = manager.ManageDiscrepancyAnalysis("", "", "");
        StringLogger.ProduceLog(DateTime.Now, sender, memberName, $"End Test");

        // Assert
        Assert.True(result.IsSuccess);
    }
}
