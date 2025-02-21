using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IDiscrepancyService
{
    List<IDiscrepancyCall> GetBillableSourceCalls(string fileLocation);
    List<IDiscrepancyCall> GetComparisonSourceCalls(string fileLocation);
}
