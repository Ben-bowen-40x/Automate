using Automate.Infrastructure.JsonToCsvService.JsonMaps;
using CsvHelper.Configuration;

namespace Automate.Infrastructure.JsonToCsvService.CsvMaps;

public class MessageMap : ClassMap<JsonMessage>
{
    public MessageMap()
    {
        int index = 0;
        Map(m=>m.Date).Index(index++).Name("Date");
        Map(m=>m.FirstName).Index(index++).Name("FirstName");
        Map(m=>m.LastName).Index(index++).Name("LastName");
        Map(m=>m.Phone).Index(index++).Name("Phone");
        Map(m=>m.Email).Index(index++).Name("Email");
        Map(m=>m.Problem).Index(index++).Name("Problem");
        Map(m=>m.Branch).Index(index++).Name("Branch");
        Map(m=>m.ReferringUrl).Index(index++).Name("ReferringUrl");
        Map(m=>m.FormPageUrl).Index(index++).Name("FormPageUrl");
        Map(m=>m.CurrentCustomer).Index(index++).Name("CurrentCustomer");
        Map(m=>m.Zip).Index(index++).Name("Zip");
    }
}
