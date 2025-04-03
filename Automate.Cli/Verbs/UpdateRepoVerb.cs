using Automate.Application.InfrastructureValueObjects;
using Automate.Application.RepoUpdate;
using Automate.Application.TypedRepoUpdate;
using Automate.Application.UpdateContacts;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.AnalyzeDiscrepancyService;
using Automate.Infrastructure.DataRetrievalFormats;
using CommandLine;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "This updates the local repo of a specified Api. This is not a REST api interface. These commands are get-only.")]
internal class UpdateRepoVerb : IVerb
{
    #region Options
    private const string VerbName = "updateRepo";
    [Option('t', "type", Required = true, HelpText = UpdateRepoHelper.RepoTypeHelpText)]
    public RepoType Type { get; set; }

    [Option('v', "valueRepo", Required = false, HelpText = "Enter the existing repository that will be updated. This repo is for value objects only and is used elsewhere. If a value is not provided, a default will be used, unless the -V --valueRepoRequired switch is used, in which case this value repo location must exist and be provided. In any case when this value is set, this value must be a CSV file. Otherwise, the application will throw.")]
    public FileInfo? ValueRepositoryCsv { get; set; }

    [Option('a', "apiRepo", Required = true, HelpText = "Enter the local repository that will be updated for the api. This repo is for api call return values and is used in soft and hard updates, but not force updates. This REQUIRED value must be a JSON file.")]
    public required FileInfo ApiRepositoryJson { get; set; }

    [Option('f', "forceUpdate", Required = false, Default = false, HelpText = "Specifies whether you would like to force a call to the api. This will pull all data from the API until all calls are exhausted, and that information will be used to refresh the domain value repo. This will only work if this application is up-to-date with the API and connected online.")]
    public bool ForceUpdate { get; set; }

    [Option('u', "hardUpdate", Required = false, Default = false, HelpText = "Specifies whether you would like to refresh the data in the local repo. If not, the local repo will be used to update the repo containing domain values (value objects) instead.")]
    public bool Update { get; set; }
    [Option('V', "valueRepoRequired", Required = false, Default = false, HelpText = "This value is used ONLY when a Csv repo file is required. Use caution when activating this switch, because the value repo file location will be required in order for this switch to work, and the Json repo file location is required and must exist as well.")]
    public bool ValueRepoRequired { get; set; }
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        #region Validate Input
        Result<FileType> verifiedJson = PathManipulation.VerifyFileType(ApiRepositoryJson);
        FileInfo repoInfo = !ApiRepositoryJson.Exists || verifiedJson.IsFailure || verifiedJson.Value != FileType.Json
            ? throw new ArgumentException($"The provided repository does not exist. This was the given repository:\n{nameof(ApiRepositoryJson)} -> {ApiRepositoryJson}")
            : ApiRepositoryJson;
        string valueRepoName = ValueRepositoryCsv is not null && ValueRepositoryCsv.Exists
            ? ValueRepositoryCsv.FullName
            : ValueRepoRequired
                ? throw new ArgumentException($"The user made the {nameof(ValueRepositoryCsv)} required, but did not provide a valid file location: {ValueRepositoryCsv}")
                : string.Empty;
        Result<FileType> verifiedCsv = PathManipulation.VerifyFileType(valueRepoName);
        string valueInfo = !File.Exists(valueRepoName) || verifiedCsv.IsFailure || verifiedCsv.Value != FileType.Csv
            ? ValueRepoRequired
                ? throw new ArgumentException($"The user made the {nameof(ValueRepositoryCsv)} required, but did not provide a valid file location, which is missing the .csv extension: {ValueRepositoryCsv}")
                : string.Empty
            : valueRepoName;
        #endregion

        #region Inform user of the chosen values
        IUserInformation inform = service.GetRequiredService<IUserInformation>();
        string chosen = $"The user chose the following values:";
        string typeMsg = $"- Repo type: \"{Type}\"";
        string valRepoMsg = $"- Value Repository location: \n\t{PathManipulation.LocationInformation(valueInfo)}";
        string valRepoRequiredMsg = $"- Whether the value repository is a required value: \n\t{ValueRepoRequired}";
        string apiRepoMsg = $"- Repository location: \n\t{PathManipulation.LocationInformation(ApiRepositoryJson.FullName)}";
        string updateMsg = $"- Whether to perform a hard update on the repositories: {Update}";
        string forceUpdateMsg = $"- Whether to perform a force update on the repositories (This will override the Hard Update option): {ForceUpdate}";
        inform.InformUser(chosen, typeMsg, valRepoMsg, valRepoRequiredMsg, apiRepoMsg, updateMsg, forceUpdateMsg);
        #endregion

        // Prepare result
        int code;

        // Execute based on the specified repository
        switch (Type)
        {
            case RepoType.Leaf:
                IRepoUpdateManager lfManager = service.GetRequiredService<IRepoUpdateManager>();
                Result leafResult = lfManager.Manage<LeafThread>(valueInfo, repoInfo.FullName, Update, ForceUpdate);
                code = DetermineReturnCode(leafResult, inform);
                break;
            case RepoType.Deprecated:
                ITypedRepoUpdateManager deprecatedManager = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result deprecatedResult = deprecatedManager.Manage<CallDbEntity>(DwhQueryType.AllCalls, DwhConnectionType.Calls, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(deprecatedResult, inform);
                break;
            case RepoType.Customers:
                ITypedRepoUpdateManager customerManager = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result customersResult = customerManager.Manage<CustSubDbEntity>(DwhQueryType.AllCustomers, DwhConnectionType.Customers, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(customersResult, inform);
                break;
            case RepoType.ContactForms:
                ITypedRepoUpdateManager formsManager = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result contactFormsResult = formsManager.Manage<WebFormEntity>(DwhQueryType.ContactForms, DwhConnectionType.ContactForms, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(contactFormsResult, inform);
                break;
            case RepoType.ContactUpdate:
                inform.InformUser($"Default values were chosen for the following choice: {RepoType.ContactUpdate}");
                IContactUpdateManager cUpdateManager = service.GetRequiredService<IContactUpdateManager>();
                UpdateResult contactUpdateResult = cUpdateManager.UpdateContacts("");

                // Inform the user what took place
                Result uploaded = contactUpdateResult.UploadedContacts;
                Result<DirectoryInfo> contactLocation = contactUpdateResult.ContactLocation;
                var c = DetermineReturnCode(uploaded, inform);
                inform.InformUser("Request: Contacts Upload");
                code = c + DetermineReturnCode(contactLocation, inform);
                inform.InformUser("Request: Contact generation");
                break;
            case var i when i == RepoType.Discrepancy || i == RepoType.Calls:
                ITypedRepoUpdateManager callsManager = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result callsResult = callsManager.Manage<DiscrepancyCallDbEntity>(DwhQueryType.Discrepancy, DwhConnectionType.Calls, repoInfo, valueInfo, ForceUpdate || Update);
                code = DetermineReturnCode(callsResult, inform);
                break;
            default:
                inform.InformUser($"No existing repository type was selected, so no execution will take place.\nEither choose an existing repository type or create a repository update functionality for the following repo selection: {Type}");
                code = ProgramErrorCodes.Error;
                break;
        };
        Environment.ExitCode = code;
        return code;
    }
    #endregion

    #region Private
    private static int DetermineReturnCode(Result result, IUserInformation inform)
    {
        string message = result.IsSuccess
            ? "Execution of this request was successful.\n"
            : $"Execution of this requrest was NOT successful. {result.Error}\n";
        int code = result.IsSuccess
            ? ProgramErrorCodes.Success
            : ProgramErrorCodes.Error;
        inform.InformUser(message);
        return code;
    }
    #endregion
}