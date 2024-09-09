using Automate.Domain.ValueObjects;

namespace Automate.Application.InfrastructureInterfaces;

public interface IMessageConvert
{
    IMessage ConvertToMessage();
}
