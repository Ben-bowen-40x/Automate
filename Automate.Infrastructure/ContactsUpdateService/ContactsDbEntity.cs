using Automate.Application.UpdateContacts;
using Automate.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Automate.Infrastructure.ContactsUpdateService;

[Keyless]
public class ContactsDbEntity
{
#nullable disable
    [Column("phone1")]
    public string Phone1 { get; set; }
#nullable enable
    [Column("phone2")]
    public string? Phone2 { get; set; }
    public Contacts Convert()
    {
        PhoneNumber number = new(Phone1);
        PhoneNumber number2 = Phone2 is null || Phone2 == string.Empty ? new(0) : new(Phone2);
        return new(number, number2);
    }
}

