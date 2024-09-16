using Automate.Application.ApiRepoUpdate;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Infrastructure.LeafClientService;
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

    [Option('v', "valueRepo", Required = false, HelpText = "Enter the existing repository that will be updated. This repo is for value objects only and is used elsewhere. If a value is not provided, a default will be used. This value must be a CSV file.")]
    public string ValueRepositoryLoc { get; set; } = string.Empty;

    [Option('a', "apiRepo", Required = false, HelpText = "Enter the local repository that will be updated for the api. This repo is for api call return values and is used in soft and hard updates, but not force updates. If a value is not provided, a default will be used. This value must be a JSON file.")]
    public string ApiRepositoryLoc { get; set; } = string.Empty;

    [Option('f', "forceUpdate", Required = false, Default = false, HelpText = "Specifies whether you would like to force a call to the api. This will pull all data from the API until all calls are exhausted, and that information will be used to refresh the domain value repo. This will only work if this application is up-to-date with the API and connected online.")]
    public bool ForceUpdate { get; set; }

    [Option('u', "hardUpdate", Required = false, Default = false, HelpText = "Specifies whether you would like to refresh the data in the local repo. If not, the local repo will be used to update the repo containing domain values (value objects) instead.")]
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
        Console.WriteLine($"- Whether to perform a force update on the repositories: {ForceUpdate}");
        Console.WriteLine($"- Whether to perform a hard update on the repositories: {Update}");

        // Validate Input
        string valueInfo = !File.Exists(ValueRepositoryLoc)
            ? LeafApiService.LeafRepoLocation
            : ValueRepositoryLoc;
        string repoInfo = !File.Exists(ApiRepositoryLoc)
            ? LeafApiService.MessageRepoLocation
            : ApiRepositoryLoc;

        // prepare result
        int code;

        // Execute based on the specified repository
        switch (Type)
        {
            case ApiType.Leaf:
                var manager = service.GetRequiredService<ILeafApiRepoUpdateManager>();
                var result = manager.Manage(valueInfo, repoInfo, Update, ForceUpdate);
                code = DetermineReturnCode(result);
                break;
            default:
                var m = service.GetRequiredService<ILeafApiRepoUpdateManager>();
                var r = m.Manage(valueInfo, repoInfo, Update, ForceUpdate);
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