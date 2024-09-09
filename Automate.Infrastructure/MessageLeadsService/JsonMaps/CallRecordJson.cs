using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.JsonMaps;

internal class CallRecordJson : IDatedRecord
{
    public NumberTypeJson? Number { get; set; }
    public bool Billable { get; set; }
    public DateTimeOffset Date { get; set; }
    public MessageCallRecord Convert()
    {
        PhoneNumber number = Number is null ? new(0) : Number.Convert();
        return new MessageCallRecord(number, Date, Billable);
    }
}