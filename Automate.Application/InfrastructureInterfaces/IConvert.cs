namespace Automate.Application.InfrastructureInterfaces;

public interface IConvert
{
    TTarget Convert<TOrigin, TTarget>();
}
public interface IConversionCompatible
{
    TTarget Convert<TTarget, TOrigin>(object? input = null);
}