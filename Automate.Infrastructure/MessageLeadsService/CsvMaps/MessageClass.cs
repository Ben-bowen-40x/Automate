using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class MessageClass : IConvert
{
    public string? Number { get; set; }
    public string? Contents { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset Date { get; set; }
    public IMessage Convert()
    {
        // Convert number
        PhoneNumber num = Number is not null
            ? new(Number)
            : new(0);

        // Convert Contents
        string contents = Contents is not null && Contents != string.Empty
            ? CsvMapsHelper.ContentsJoined(Contents)
            : string.Empty;

        // Convert Source
        string source = Source is not null && Source != string.Empty
            ? Source
            : string.Empty;

        // Cast new message into IMessage
        IMessage rMsg = new Message(num, Date, contents, source);

        return rMsg;
    }

    public IMessage Convert<MessageClass, IMessage>()
    {
        return (IMessage)Convert();
    }
}