using Automate.Application.InfrastructureInterfaces;

namespace Automate.Translation.InfrastructureInterfaces.Call;

public interface ICallZoneStr : IPhoneNumberCompatible
{
    public long NumberLong { get; set; }
    public string? Billable { get; set; }
    public DateTime? Date { get; set; }
    public string? TimeZone { get; set; }
}
