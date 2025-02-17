using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface ITypedDiscrepancyService
{
    List<IDiscrepancyCall> GetCalls<T>(FileInfo fileLocation) where T : IConvert;
}