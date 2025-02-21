using Automate.Translation.ContactTranslate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Automate.Infrastructure.ContactsUpdateService;

[Keyless]
public class ContactsDbEntity : IContactsEntity
{
#nullable disable
    [Column("phone1")]
    public string Phone1 { get; set; }
#nullable enable
    [Column("phone2")]
    public string? Phone2 { get; set; }
}
