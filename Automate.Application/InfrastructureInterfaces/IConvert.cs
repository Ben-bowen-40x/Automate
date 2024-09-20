using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IConvert
{
    TTarget Convert<TOrigin, TTarget>();
}
public interface IPhoneNumberCompatible
{
    PhoneNumber Number { get; }
}