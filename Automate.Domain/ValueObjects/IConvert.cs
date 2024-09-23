namespace Automate.Domain.ValueObjects;

public interface IConvert
{
    TTarget Convert<TOrigin, TTarget>();
}
