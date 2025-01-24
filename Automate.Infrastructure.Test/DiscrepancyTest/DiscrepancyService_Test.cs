using Automate.Domain.ValueObjects;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
namespace Automate.Infrastructure.Test.DiscrepancyTest;

public class DiscrepancyService_Test(IDwhTestSettings settings)
{
    private readonly IDwhTestSettings _settings = settings;
    [
        Theory,
        InlineData(true),
    //InlineData(false),
    ]
    public void DiscrepancyService_RetrievesInfoProperly(bool queryDb)
    {
        // Assemble
        DiscrepancyService service = new(_settings) { QueryDb = queryDb };

        // Act
        List<DiscrepancyCall> billableCalls = service.GetBillableSourceCalls();
        service.QueryDb = queryDb; // This needs to be done because GetBillableSourceCalls will change this value
        List<DiscrepancyCall> comparisonCalls = service.GetComparisonSourceCalls();

        // Assert
        Assert.NotEmpty(billableCalls);
        Assert.NotEmpty(comparisonCalls);
    }
}
