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
internal class CallRecordJsonReader : IDatedRecord
{
    public NumberTypeJson? Number { get; set; }
    public string? Billable { get; set; }
    public DateTimeOffset Date { get; set; }
    public MessageCallRecord Convert()
    {
        PhoneNumber number = Number is null ? new(0) : Number.Convert();
        bool billable = Billable is not null && Billable == "billable";
        return new(number, Date, billable);
    }
}