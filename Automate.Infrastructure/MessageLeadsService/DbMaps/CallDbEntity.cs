using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Automate.Domain.ValueObjects;
using Automate.Application.InfrastructureInterfaces;

namespace Automate.Infrastructure.MessageLeadsService.DbMaps;

[Keyless]
public class CallDbEntity : IPhoneNumberCompatible
{
    [Column("contact_number_clean")]
    public long Num { get; set; }
    [Column("sale_billable")]
    public string? Billable { get; set; }
    [Column("called_at_utc")]
    public DateTime? Date { get; set; }
    [Column("time_zone")]
    public string? TimeZone { get; set; }
    private PhoneNumber? _num;
    public PhoneNumber Number => _num ??= new(Num);

    public MessageCallRecord Convert()
    {
        PhoneNumber num = new(Num);
        bool billable = Billable is not null & Billable == "billable";
        TimeSpan timeZone = TimeZone is not null & TimeSpan.TryParse(TimeZone, out TimeSpan tzResult) ? tzResult : new(0);
        DateTime dateInter = Date is null ? DateTime.MinValue : (DateTime)Date;
        DateTimeOffset date = new(dateInter, timeZone);
        var record = new MessageCallRecord(num, date, billable);
        return record;
    }

}