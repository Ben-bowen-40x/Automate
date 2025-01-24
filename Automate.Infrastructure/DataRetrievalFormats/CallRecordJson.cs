using Automate.Translation.CallTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

internal class CallRecordJson : ICallType
{
    public NumberType? Number { get; set; }
    public bool Billable { get; set; }
    public DateTimeOffset Date { get; set; }

}
