using Automate.Application.ApiRepoUpdate;
using Automate.Cli.Verbs.VerbHelper;
using CommandLine;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "This updates the local repo of a specified Api. Obviously, this is get-only.")]
internal class UpdateApiRepoVerb : IVerb
{
    private const string VerbName = "updateApiRepo";

    #region Options
    [Option('t', "type", Required = true, HelpText = UpdateApiRepoHelper.ApiTypeHelpText)]
    public ApiType Type { get; set; }

    [Option('v', "valueRepo", Required = false, HelpText = "Enter the existing repository that will be updated. This repo is for value objects and is used elsewhere.")]
    public string ValueRepositoryLoc { get; set; } = string.Empty;

    [Option('l', "apiRepo", Required = false, HelpText = "Enter the local repository that will be updated for the api. This repo is for api call return values and is used as a backup.")]
    public string ApiRepositoryLoc { get; set; } = string.Empty;

    [Option('u', "updateRepo", Required = true, Default = false, HelpText = "Specifies whether you would like a hard reset of both the Api repo AND the value repo.")]
    public bool Update { get; set; }
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        // Inform user of the chosen values
        Console.WriteLine($"The user chose the following values:");
        Console.WriteLine($"- Api type: \"{Type}\"");
        Console.WriteLine($"- Value Repository location: \n    {DirectoryManipulation.LocationInformation(ValueRepositoryLoc)}");
        Console.WriteLine($"- Api Repository location: \n    {DirectoryManipulation.LocationInformation(ApiRepositoryLoc)}");
        Console.WriteLine($"- Whether to update the repository: {Update}");

        // Validate Input
        string valueInfo = !File.Exists(ValueRepositoryLoc)
            ? ""
            : ValueRepositoryLoc;
        string repoInfo = !File.Exists(ApiRepositoryLoc)
            ? ""
            : ApiRepositoryLoc;

        // prepare result
        int code;

        // Execute based on the specified repository
        switch (Type)
        {
            case ApiType.Leaf:
                var manager = service.GetRequiredService<ILeafApiRepoUpdateManager>();
                var result = manager.Manage(valueInfo, repoInfo, Update);
                code = DetermineReturnCode(result);
                break;
            default:
                var m = service.GetRequiredService<ILeafApiRepoUpdateManager>();
                var r = m.Manage(valueInfo, repoInfo, Update);
                code = DetermineReturnCode(r);
                break;
        };

        return code;
    }
    #endregion

    #region Private
    private static int DetermineReturnCode(Result result)
    {
        if (result.IsSuccess)
            return ProgramErrorCodes.Success;
        return ProgramErrorCodes.Error;
    }
    #endregion
}