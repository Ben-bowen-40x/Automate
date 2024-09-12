using Automate.Domain.ValueObjects;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

internal class MessageMapRW : ClassMap<IMessage>
{
    MessageMapRW()
    {
        int index = 0;
        Map(m => m.Number.Number).Index(index++).Name("Number");
        Map(m => m.Date).Index(index++).Name("Date");
        Map(m => m.Source).Index(index++).Name("Source");
        Map(m => m.Contents).Index(index++).Name("Contents");
    }
}
