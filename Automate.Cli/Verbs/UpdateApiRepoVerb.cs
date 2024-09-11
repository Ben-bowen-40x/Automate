using Automate.Cli.Verbs.VerbHelper;
using CommandLine;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "This updates the local repo of a specified Api.")]
internal class UpdateApiRepoVerb : IVerb
{
    private const string VerbName = "updateApiRepo";

    #region Options
    [Option('t', "type", Required = true, HelpText = UpdateApiRepoHelper.ApiTypeHelpText)]
    public ApiType Type { get; set; }

    [Option('r', "repo", Required = true, HelpText = "Enter the existing repository that will be updated. A value must be provided, and it must be a file on this machine.")]
    public string RepositoryLoc { get; set; } = string.Empty;

    [Option('u', "updateRepo", Required = true, HelpText = "Specify whether you would like the locally saved repository to be updated by calling the api.")]
    public bool Update { get; set; }
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        // Inform user of the chosen values
        Console.WriteLine($"The user chose the following values:");
        Console.WriteLine($"- Api type: \"{Type}\"");
        if (RepositoryLoc != string.Empty)
            Console.WriteLine($"- Repository location: \n    {DirectoryManipulation.LocationInformation(RepositoryLoc)}");
        Console.WriteLine($"- Whether to update the repository: {Update}");

        // 

        return ProgramErrorCodes.Success;
    }
    #endregion
}