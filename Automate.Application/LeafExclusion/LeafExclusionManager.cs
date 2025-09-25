using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.LeafExclusion;

public class LeafExclusionManager(ILeafExcludeService exclude) : ILeafExclusionManager
{
    private readonly ILeafExcludeService _excludeService = exclude;
    public Result Manage(FileInfo leafRepo, FileInfo output)
    {
        List<ILeafThread> consentList = _excludeService.GetConsentList(leafRepo);
        List<PhoneNumber> consentNumbers = _excludeService.GetConsentNumbers(consentList);
        Result saveResult = _excludeService.Save(consentNumbers, output);
        return saveResult;
    }
}
