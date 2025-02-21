using Automate.Application.InfrastructureInterfaces;

namespace Automate.Translation.CallTranslate;

public interface ICallDateTimeInUTC : IPhoneNumberCompatible
{
    public long NumberLong { get; set; }
    public string? Billable { get; set; }
    public DateTime? Date { get; set; }
    public string? TimeZone { get; set; }
}
