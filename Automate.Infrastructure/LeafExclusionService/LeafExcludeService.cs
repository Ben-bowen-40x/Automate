using Automate.Application.InfrastructureInterfaces;
using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.JsonManipulationService;
using Automate.Translation.PhoneNumTranslate;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.LeafExclusionService;

public class LeafExcludeService : ILeafExcludeService
{
    public List<ILeafThread> GetConsentList(FileInfo fileInfo)
    {
        Result<List<LeafThread>> threadResult = JsonService.ReadFile<LeafThread>(fileInfo);
        List<LeafThread> threads = threadResult.IsSuccess
            ? threadResult.Value
            : throw new Exception(threadResult.Error);

        List<ILeafThread> consent = [.. threads.Where(t => t.Prospect is not null && t.Prospect.Consent == false)];
        return consent;
    }

    public List<PhoneNumber> GetConsentNumbers(List<ILeafThread> optOutThreads)
    {
        List<PhoneNumber> result = [.. optOutThreads
            .Select(t =>
                {
                    string cell = t.Prospect is null || t.Prospect.Cellphone is null
                        ? "0"
                        : t.Prospect!.Cellphone!;
                    PhoneNumber phoneNumber = PhoneNumberTranslate.Translate(cell);
                    return phoneNumber;
                })
            .Where(p => p.Number != PhoneNumber.Default)];
        return result;
    }

    public Result Save(List<PhoneNumber> numbers, FileInfo location)
    {
        Result result = CsvService.Write(numbers.Select(n => n.Number).ToList(), location);
        return result;
    }
}
