using CSharpFunctionalExtensions;

namespace Automate.Domain.SolutionFunctionality;

public interface IUserInformation
{
    public Result<T> InformUser<T>(T message);
    public Result<T> InformUser<T>(params T[] messages);
}
