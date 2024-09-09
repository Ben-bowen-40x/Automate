using Automate.Domain.ValueObjects;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.DatabaseService;
using Microsoft.Extensions.Configuration;
namespace Automate.Infrastructure.Test.DiscrepancyTest;

public class DiscrepancyService_Test(IDwhSettings settings)
{
    [
        Theory,
        InlineData(true),
    //InlineData(false),
    ]
    public void DiscrepancyService_RetrievesInfoProperly(bool queryDb)
    {
        // Assemble
        DiscrepancyService service = new(settings) { QueryDb = queryDb };

        // Act
        List<DiscrepancyCall> billableCalls = service.GetBillableSourceCalls();
        service.QueryDb = queryDb; // This needs to be done because GetBillableSourceCalls will change this value
        List<DiscrepancyCall> comparisonCalls = service.GetComparisonSourceCalls();

        // Assert
        Assert.NotEmpty(billableCalls);
        Assert.NotEmpty(comparisonCalls);
    }
}
