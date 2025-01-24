using Automate.Domain.ValueObjects;
using Automate.Translation.InfrastructureInterfaces.Contact;
using Automate.Translation.ValueObjectsTranslations;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Automate.Infrastructure.MessageLeadsService.DbMaps;

[Keyless]
public class WebFormEntity : ClassMap<WebFormEntity>, IContactFormTyped
{
    public const string DateStr = "Date";
    public const string FName = "First Name";
    public const string LName = "Last Name";
    public const string Ph = "Phone";
    public const string Eml = "Email";
    public const string Prob = "Problem";
    public const string Brnch = "Branch";
    public const string Refer = "Referring URL";
    public const string Form = "Form Page URL";
    public const string Cust = "Current Customer";
    public const string ZCode = "Zip";
    WebFormEntity()
    {
        int index = 0;
        Map(m => m.Date).Index(index++).Name(DateStr);
        Map(m => m.FirstName).Index(index++).Name(FName);
        Map(m => m.LastName).Index(index++).Name(LName);
        Map(m => m.Number.Number).Index(index++).Name(Ph);
        Map(m => m.Email).Index(index++).Name(Eml);
        Map(m => m.Problem).Index(index++).Name(Prob);
        Map(m => m.Branch).Index(index++).Name(Brnch);
        Map(m => m.ReferringUrl).Index(index++).Name(Refer);
        Map(m => m.FormPageUrl).Index(index++).Name(Form);
        Map(m => m.CurrentCustomer).Index(index++).Name(Cust);
        Map(m => m.Zip).Index(index++).Name(ZCode);
    }

    [Column(DateStr)]
    [Name(DateStr)]
    public DateTime Date { get; set; }
    [Column(FName)]
    [Name(FName)]
    public string? FirstName { get; set; }
    [Column(LName)]
    [Name(LName)]
    public string? LastName { get; set; }
    [Column(Ph)]
    [Name(Ph)]
    public string? Phone { get; set; }
    [Column(Eml)]
    [Name(Eml)]
    public string? Email { get; set; }
    [Column(Prob)]
    [Name(Prob)]
    public string? Problem { get; set; }
    [Column(Brnch)]
    [Name(Brnch)]
    public string? Branch { get; set; }
    [Column(Refer)]
    [Name(Refer)]
    public string? ReferringUrl { get; set; }
    [Column(Form)]
    [Name(Form)]
    public string? FormPageUrl { get; set; }
    [Column(Cust)]
    [Name(Cust)]
    public string? CurrentCustomer { get; set; }
    [Column(ZCode)]
    [Name(ZCode)]
    public string? Zip { get; set; }

    private PhoneNumber? _number;
    public PhoneNumber Number => _number ??= PhoneNumberTranslate.Convert(Phone);
}
