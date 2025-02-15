using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.Test.TestConfigurations;
namespace Automate.Infrastructure.Test.DiscrepancyTest;

public class DiscrepancyService_Test
{
    private readonly IInfrastructureTestSettings _settings = new InfraTestConfiguration().TestSettings;
    [Fact]
    public void DiscrepancyService_RetrievesInfoProperly()
    {
        // Assemble
        DiscrepancyService service = new(_settings as IDwhSettings);

        // Act
        List<IDiscrepancyCall> billableCalls = service.GetBillableSourceCalls();
        List<IDiscrepancyCall> comparisonCalls = service.GetComparisonSourceCalls();

        // Assert
        Assert.NotEmpty(billableCalls);
        Assert.NotEmpty(comparisonCalls);
    }
    [Fact]
    public void DiscrepancyQuery_RetrievesProperly()
    {
        // Assemble
        var q = new RawQuery(_settings as IRawQuerySettings);

        // Act
        IQuery action = q.DiscrepancyQuery();
        action.AppendWhere(_settings.Discrepancy2!);

        // Assert
        Assert.NotNull(action);
    }
}
