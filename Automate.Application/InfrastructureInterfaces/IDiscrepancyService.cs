using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IDiscrepancyService
{
    List<DiscrepancyCall> GetBillableSourceCalls(string fileLocation);
    List<DiscrepancyCall> GetComparisonSourceCalls(string fileLocation);
}
