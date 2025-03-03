using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class LeasedMessage : IMsgStrsSpecialExtras, IConvert
{
    [Name("Date Received")]
    public string? Date { get; set; }
    [Name("Date Completed")]
    public string? TimeZoneInfo { get; set; }
    [Name("Phone Number")]
    public string? PhoneNumStr { get; set; }
    [Name("Message Chain")]
    public string? Contents { get; set; }
    [Name("Branch")]
    public string? Source { get; set; }
    [Name("Was it a Lead?")]
    public string? Lead { get; set; }

    public IMessage Convert<IMsgStrsSpecialExtras, IMessage>()
    {
        Translation.MessageTranslate.IMsgStrsSpecialExtras @this = this;
        return (IMessage)@this.Translate();
    }
}
