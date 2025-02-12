using Automate.Domain.ValueObjects;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.Test.TestConfigurations;
namespace Automate.Infrastructure.Test.DiscrepancyTest;

public class DiscrepancyService_Test
{
    public DiscrepancyService_Test()
    {
        _settings = new InfraTestConfiguration().TestSettings;
    }
    private readonly IDwhTestSettings _settings;
    [
        Theory
        (Skip = "This is not working right now")
        ,
        InlineData(true),
        //InlineData(false),
    ]
    public void DiscrepancyService_RetrievesInfoProperly(bool queryDb)
    {
        // Assemble
        DiscrepancyService service = new(_settings) { QueryDb = queryDb };

        // Act
        List<IDiscrepancyCall> billableCalls = service.GetBillableSourceCalls();
        service.QueryDb = queryDb; // This needs to be done because GetBillableSourceCalls will change this value
        List<IDiscrepancyCall> comparisonCalls = service.GetComparisonSourceCalls();

        // Assert
        Assert.NotEmpty(billableCalls);
        Assert.NotEmpty(comparisonCalls);
    }
}
