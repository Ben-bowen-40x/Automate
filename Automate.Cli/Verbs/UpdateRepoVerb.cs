using Automate.Application.InfrastructureValueObjects;
using Automate.Application.RepoUpdate;
using Automate.Application.TypedRepoUpdate;
using Automate.Application.UpdateContacts;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.DataRetrievalFormats;
using CommandLine;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "This updates the local repo of a specified Api. Obviously, this is get-only.")]
internal class UpdateRepoVerb : IVerb
{
    private const string VerbName = "updateRepo";

    #region Options
    [Option('t', "type", Required = true, HelpText = UpdateRepoHelper.RepoTypeHelpText)]
    public RepoType Type { get; set; }

    [Option('v', "valueRepo", Required = false, HelpText = "Enter the existing repository that will be updated. This repo is for value objects only and is used elsewhere. If a value is not provided, a default will be used. This value must be a CSV file.")]
    public FileInfo? ValueRepositoryCsv { get; set; }

    [Option('a', "apiRepo", Required = true, HelpText = "Enter the local repository that will be updated for the api. This repo is for api call return values and is used in soft and hard updates, but not force updates. If a value is not provided, a default will be used. This value must be a JSON file.")]
    public required FileInfo ApiRepositoryJson { get; set; }

    [Option('f', "forceUpdate", Required = false, Default = false, HelpText = "Specifies whether you would like to force a call to the api. This will pull all data from the API until all calls are exhausted, and that information will be used to refresh the domain value repo. This will only work if this application is up-to-date with the API and connected online.")]
    public bool ForceUpdate { get; set; }

    [Option('u', "hardUpdate", Required = false, Default = false, HelpText = "Specifies whether you would like to refresh the data in the local repo. If not, the local repo will be used to update the repo containing domain values (value objects) instead.")]
    public bool Update { get; set; }
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        // Validate Input
        Result<FileType> apiJson = PathManipulation.VerifyType(ApiRepositoryJson);
        FileInfo repoInfo = !ApiRepositoryJson.Exists || apiJson.IsFailure || apiJson.Value != FileType.Json
            ? throw new ArgumentException($"The provided repository does not exist. This was the given repository:\n{nameof(ApiRepositoryJson)} -> {ApiRepositoryJson}")
            : ApiRepositoryJson;
        string valueRepoName = ValueRepositoryCsv is not null ? ValueRepositoryCsv.FullName : string.Empty;
        Result<FileType> valueCsv = PathManipulation.VerifyType(valueRepoName);
        string valueInfo = !File.Exists(valueRepoName) || valueCsv.IsFailure || valueCsv.Value != FileType.Csv
            ? string.Empty
            : valueRepoName;

        // Inform user of the chosen values
        Console.WriteLine($"The user chose the following values:");
        Console.WriteLine($"- Repo type: \"{Type}\"");
        Console.WriteLine($"- Value Repository location: \n\t{PathManipulation.LocationInformation(valueInfo)}");
        Console.WriteLine($"- Repository location: \n\t{PathManipulation.LocationInformation(ApiRepositoryJson.FullName)}");
        Console.WriteLine($"- Whether to perform a hard update on the repositories: {Update}");
        Console.WriteLine($"- Whether to perform a force update on the repositories (This will override the Hard Update option): {ForceUpdate}");

        // Prepare result
        int code;

        // Execute based on the specified repository
        switch (Type)
        {
            case RepoType.Leaf:
                IRepoUpdateManager manager = service.GetRequiredService<IRepoUpdateManager>();
                Result result = manager.Manage<LeafThread>(valueInfo, repoInfo.FullName, Update, ForceUpdate);
                code = DetermineReturnCode(result);
                break;
            case RepoType.Deprecated:
                ITypedRepoUpdateManager manage = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result resul = manage.Manage<CallDbEntity>(DwhQueryType.AllCalls, DwhConnectionType.Calls, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(resul);
                break;
            case RepoType.Customers:
                ITypedRepoUpdateManager manag = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result resu = manag.Manage<CustSubDbEntity>(DwhQueryType.AllCustomers, DwhConnectionType.Customers, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(resu);
                break;
            case RepoType.ContactForms:
                ITypedRepoUpdateManager mana = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result res = mana.Manage<WebFormEntity>(DwhQueryType.ContactForms, DwhConnectionType.ContactForms, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(res);
                break;
            case RepoType.ContactUpdate:
                Console.WriteLine($"Default values were chosen for the following choice: {RepoType.ContactUpdate}");
                IContactUpdateManager man = service.GetRequiredService<IContactUpdateManager>();
                UpdateResult re = man.UpdateContacts("");

                // Inform the user what took place
                Result uploaded = re.UploadedContacts;
                Result<DirectoryInfo> contactLocation = re.ContactLocation;
                _ = DetermineReturnCode(uploaded);
                Console.WriteLine("Request: Contacts Upload");
                code = DetermineReturnCode(contactLocation);
                Console.WriteLine("Request: Contact generation");
                break;
            case var i when i == RepoType.Discrepancy || i == RepoType.Calls:
                ITypedRepoUpdateManager ma = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result r = ma.Manage<DiscrepancyCallDbEntity>(DwhQueryType.Discrepancy, DwhConnectionType.Calls, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(r);
                break;
            default:
                Console.WriteLine($"No existing repository type was selected, so no execution will take place.\nEither choose an existing repository type or create a repository update functionality for the following repo selection: {Type}");
                code = ProgramErrorCodes.Error;
                break;
        };

        return code;
    }
    #endregion

    #region Private
    private static int DetermineReturnCode(Result result)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine("Execution of this request was successful.");
            return ProgramErrorCodes.Success;
        }
        Console.WriteLine($"Execution of this request was NOT successful. {result.Error}");
        return ProgramErrorCodes.Error;
    }
    #endregion
}