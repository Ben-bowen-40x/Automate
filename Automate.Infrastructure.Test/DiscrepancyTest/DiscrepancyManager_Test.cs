using Automate.Application.Discrepancy;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.ReportingService;
using Automate.Infrastructure.Retrieval;
using Automate.Infrastructure.Test.TestConfigurations;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.Test.DiscrepancyTest;

public class DiscrepancyManager_Test
{
    private readonly IInfrastructureTestSettings _settings = new InfraTestConfiguration().TestSettings;

    #region Deprecated
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
    #endregion

    #region TypedDiscrepancyManager
    [Fact]
    public void TypedDiscrepancyManager_Executes()
    {
        // Assemble
        TypedDiscrepancyManager manger = new(new DiscrepancyService(_settings), new ReportServiceSingleton());
        FileInfo billedCalls = FolderFinder.GetLocalFile(nameof(Infrastructure), @".info\Discrepancy", "Discrepancy.csv");
        FileInfo comparison = FolderFinder.GetLocalFile(nameof(Infrastructure), @".info\Discrepancy\LocalRepo", "Discrepancy.json");
        FileInfo report = FolderFinder.GetLocalFile(nameof(Infrastructure), @".info\Discrepancy\Test", "TestDiscrepancyReport.csv");

        // Act
        Result<FileInfo> result = manger.Manage<DiscrepancySourceLeadsCsvColumns, DiscrepancyJson>(billedCalls, comparison, report);

        // Assert
        Assert.True(result.IsSuccess);
    }
    #endregion
}
