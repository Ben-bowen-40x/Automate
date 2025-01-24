using Automate.Translation.ContactTranslate;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.DataRetrievalFormats;

public class MessageMap : ClassMap<JsonMessage>, IContactFormString
{
    public MessageMap()
    {
        int index = 0;
        Map(m => m.Date).Index(index++).Name(WebFormEntity.DateStr);
        Map(m => m.FirstName).Index(index++).Name(WebFormEntity.FName);
        Map(m => m.LastName).Index(index++).Name(WebFormEntity.LName);
        Map(m => m.Phone).Index(index++).Name(WebFormEntity.Ph);
        Map(m => m.Email).Index(index++).Name(WebFormEntity.Eml);
        Map(m => m.Problem).Index(index++).Name(WebFormEntity.Prob);
        Map(m => m.Branch).Index(index++).Name(WebFormEntity.Brnch);
        Map(m => m.ReferringUrl).Index(index++).Name(WebFormEntity.Refer);
        Map(m => m.FormPageUrl).Index(index++).Name(WebFormEntity.Form);
        Map(m => m.CurrentCustomer).Index(index++).Name(WebFormEntity.Cust);
        Map(m => m.Zip).Index(index++).Name(WebFormEntity.ZCode);
    }
    [Name(WebFormEntity.DateStr)]
    public string? Date { get; set; }
    [Name(WebFormEntity.FName)]
    public string? FirstName { get; set; }
    [Name(WebFormEntity.LName)]
    public string? LastName { get; set; }
    [Name(WebFormEntity.Ph)]
    public string? Phone { get; set; }
    [Name(WebFormEntity.Eml)]
    public string? Email { get; set; }
    [Name(WebFormEntity.Prob)]
    public string? Problem { get; set; }
    [Name(WebFormEntity.Brnch)]
    public string? Branch { get; set; }
    [Name(WebFormEntity.Refer)]
    public string? ReferringUrl { get; set; }
    [Name(WebFormEntity.Form)]
    public string? FormPageUrl { get; set; }
    [Name(WebFormEntity.Cust)]
    public string? CurrentCustomer { get; set; }
    [Name(WebFormEntity.ZCode)]
    public string? Zip { get; set; }

}
