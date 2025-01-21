using Automate.Translation.InfrastructureInterfaces.Message;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

public class MessageClass : IMsgStrDateTimeOffset
{
    public string? Number { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Contents { get; set; }
    public string? Source { get; set; }
}