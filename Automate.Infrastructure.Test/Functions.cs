using Automate.Domain.SolutionFunctionality;

namespace Automate.Infrastructure.Test;

public static class Functions
{
    public static FileInfo TestFile(string location, string file) => FolderFinder.GetLocalFile(nameof(Infrastructure), location, file);
    public static readonly FileInfo pathtofile = TestFile(@".info\ApiRepos\LeafTesting\", "Messages_Test.csv");
}
