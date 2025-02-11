using Automate.Application.InfrastructureValueObjects;
using Automate.Application.RepoUpdate;
using Automate.Application.TypedRepoUpdate;
using Automate.Application.UpdateContacts;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.ValueObjects;
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
    public string ValueRepositoryCsv { get; set; } = string.Empty;

    [Option('a', "apiRepo", Required = false, HelpText = "Enter the local repository that will be updated for the api. This repo is for api call return values and is used in soft and hard updates, but not force updates. If a value is not provided, a default will be used. This value must be a JSON file.")]
    public string ApiRepositoryJson { get; set; } = string.Empty;

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
        Console.WriteLine($"- Repo type: \"{Type}\"");
        Console.WriteLine($"- Value Repository location: \n\t{PathManipulation.LocationInformation(ValueRepositoryCsv)}");
        Console.WriteLine($"- Repository location: \n\t{PathManipulation.LocationInformation(ApiRepositoryJson)}");
        Console.WriteLine($"- Whether to perform a hard update on the repositories: {Update}");
        Console.WriteLine($"- Whether to perform a force update on the repositories (This will override the Hard Update option): {ForceUpdate}");

        // Validate Input
        Result<FileType> valueCsv = PathManipulation.VerifyType(ValueRepositoryCsv);
        Result<FileType> apiJson = PathManipulation.VerifyType(ApiRepositoryJson);
        string valueInfo = !File.Exists(ValueRepositoryCsv) || valueCsv.IsFailure || valueCsv.Value != FileType.Csv
            ? ""
            : ValueRepositoryCsv;
        string repoInfo = !File.Exists(ApiRepositoryJson) || apiJson.IsFailure || apiJson.Value != FileType.Json
            ? ""
            : ApiRepositoryJson;

        // Prepare result
        int code;

        // Execute based on the specified repository
        switch (Type)
        {
            case RepoType.Leaf:
                var manager = service.GetRequiredService<IRepoUpdateManager>();
                var result = manager.Manage(valueInfo, repoInfo, Update, ForceUpdate);
                code = DetermineReturnCode(result);
                break;
            case RepoType.Calls:
                var manage = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result resul = manage.Manage<CallDbEntity>(DwhQueryType.AllCalls, DwhConnectionType.Calls, ApiRepositoryJson, ForceUpdate || Update);
                code = DetermineReturnCode(resul);
                break;
            case RepoType.Customers:
                var manag = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result resu = manag.Manage<CustSubDbEntity>(DwhQueryType.AllCustomers, DwhConnectionType.Customers, ApiRepositoryJson, ForceUpdate || Update);
                code = DetermineReturnCode(resu);
                break;
            case RepoType.ContactForms:
                var mana = service.GetRequiredService<ITypedRepoUpdateManager>();
                Result res = mana.Manage<WebFormEntity>(DwhQueryType.ContactForms, DwhConnectionType.ContactForms, ApiRepositoryJson, ForceUpdate || Update);
                code = DetermineReturnCode(res);
                break;
            case RepoType.ContactUpdate:
                Console.WriteLine($"Default values were chosen for the following choice: {RepoType.ContactUpdate}");
                var man = service.GetRequiredService<IContactUpdateManager>();
                UpdateResult re = man.UpdateContacts("");
                
                // Inform the user what took place
                var uploaded = re.UploadedContacts;
                var contactLocation = re.ContactLocation;
                _ = DetermineReturnCode(uploaded);
                Console.WriteLine("Request: Contacts Upload");
                code = DetermineReturnCode(contactLocation);
                Console.WriteLine("Request: Contact generation");
                break;
            default:
                var m = service.GetRequiredService<IRepoUpdateManager>();
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
        {
            Console.WriteLine("Execution of this request was successful.");
            return ProgramErrorCodes.Success;
        }
        Console.WriteLine($"Execution of this request was NOT successful. {result.Error}");
        return ProgramErrorCodes.Error;
    }
    #endregion
}