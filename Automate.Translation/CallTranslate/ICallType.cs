using Automate.Domain.ValueObjects;

namespace Automate.Translation.CallTranslate;

public interface ICallType : IDatedRecord
{
    NumberType? Number { get; set; }
    bool Billable { get; set; }
}
