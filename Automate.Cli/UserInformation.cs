using Automate.Domain.SolutionFunctionality;
using CSharpFunctionalExtensions;

namespace Automate.Cli;

internal class UserInformation : IUserInformation
{
    public Result<T> InformUser<T>(T message)
    {
        try
        {
            Console.WriteLine(message);
        }
        catch (Exception ex)
        { return Result.Failure<T>(ex.Message); }
        return Result.Success(message);
    }

    public Result<T> InformUser<T>(params T[] messages)
    {
        List<Result<T>> list = new(messages.Length);
        foreach (T msg in messages)
        {
            Result<T> result = InformUser(msg);
            list.Add(result);
        }
        Result combination = Result.Combine(list);
        return combination.IsSuccess
            ? list[0]
            : Result.Failure<T>(combination.Error);
    }
}
