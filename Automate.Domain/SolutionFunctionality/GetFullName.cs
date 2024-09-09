using System.Runtime.CompilerServices;

namespace Automate.Domain.SolutionFunctionality;

public class GetFullName
{
    public static string GetMemberName(object origin, [CallerMemberName] string memberName = "")
    {
        string fullName = origin.GetType().FullName!;
        string result = memberName == "" ? fullName : fullName + "." + memberName;
        return result;
    }
}
