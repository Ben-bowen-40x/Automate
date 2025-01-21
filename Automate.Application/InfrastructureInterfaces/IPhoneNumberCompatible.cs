using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;


public interface IPhoneNumberCompatible
{
    PhoneNumber Number { get; }
}