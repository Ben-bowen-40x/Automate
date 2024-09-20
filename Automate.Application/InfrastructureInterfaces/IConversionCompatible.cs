namespace Automate.Application.InfrastructureInterfaces;

public interface IConversionCompatible
{
    TTarget Convert<TTarget, TOrigin>(object? input = null);
}