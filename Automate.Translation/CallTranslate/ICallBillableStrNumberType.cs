using Automate.Domain.ValueObjects;

namespace Automate.Translation.CallTranslate;

public interface ICallBillableStrNumberType : IDatedRecord
{
    NumberType? Number { get; set; }
    string? Billable { get; set; }
}
