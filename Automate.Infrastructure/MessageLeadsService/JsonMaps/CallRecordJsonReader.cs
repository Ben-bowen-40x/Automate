using Automate.Translation.CallTranslate;

namespace Automate.Infrastructure.MessageLeadsService.JsonMaps;

internal class CallRecordJsonReader : ICallBillableStrNumberType
{
    public NumberType? Number { get; set; }
    public string? Billable { get; set; }
    public DateTimeOffset Date { get; set; }    
}