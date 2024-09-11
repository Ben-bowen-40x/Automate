using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

internal class MessageMapRW : ClassMap<IMessage>
{
    MessageMapRW()
    {
        int index = 0;
        Map(m => m.Number).Index(index++).Name("Number");
        Map(m => m.Date).Index(index++).Name("Date");
        Map(m => m.Source).Index(index++).Name("Source");
        Map(m => m.Contents).Index(index++).Name("Contents");
    }
}
public class MessageClass : IMessageConvert
{
    public PhoneNumber? Number { get; set; }
    public string? Contents { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset Date { get; set; }
    public IMessage ConvertToMessage()
    {
        // Convert number
        PhoneNumber num = Number is not null
            ? Number
            : new(0);

        // Convert Contents
        string contents = Contents is not null && Contents != string.Empty
            ? Contents
            : string.Empty;

        // Convert Source
        string source = Source is not null && Source != string.Empty
            ? Source
            : string.Empty;

        return new Message(num, Date, contents, source); 
    }
}