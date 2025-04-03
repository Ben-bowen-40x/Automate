using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Automate.Application.UpdateContacts;
using Automate.Domain.SolutionFunctionality;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.ValueObjects;

namespace Automate.Cli.Verbs;

[Verb(UpdateContacts, HelpText = "Choose to update the contacts list. This either produces or accepts an existing folder location where the contact list files will be housed.")]
internal class ContactUpdateVerb : IVerb
{
    private const string UpdateContacts = "updateContacts";

    #region Options
    [Option('r', "report", Required = false, HelpText = "Enter the full name of the directory where the contact files should be deposited.")]
    public string ReportDirectory { get; set; } = string.Empty;
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        // Inform the user what's going on
        IUserInformation inform = service.GetRequiredService<IUserInformation>();
        inform.InformUser($"For the following option, \"{nameof(ReportDirectory)}\" -- {PathManipulation.LocationInformation(ReportDirectory)}");

        // Validate the user's information
        bool directoryNull = ReportDirectory != string.Empty;
        bool directoryExists = Directory.Exists(ReportDirectory);
        if (directoryExists && !directoryNull)
            Directory.CreateDirectory(ReportDirectory);

        // Execute
        var manager = service.GetRequiredService<IContactUpdateManager>();
        UpdateResult result = manager.UpdateContacts(ReportDirectory);

        StringLogger.NameLog(DateTime.Now, UpdateContacts);

        int code = DetermineReturnCode(result, directoryExists, inform);
        Environment.ExitCode = code;
        return code;
    }
    #endregion

    #region Private
    private static int DetermineReturnCode(UpdateResult result, bool directoryExists, IUserInformation inform)
    {
        const string generated = "The contacts where generated.";
        const string nGenerated = "At least one contact was not generated";

        const string uploaded = "The contacts were successfully uploaded";
        const string nUploaded = "The contacts were not successfully uploaded";

        string exists = result.ContactLocation.IsSuccess
            ? $"The directory containing the contacts given by the user exists. Here is the directory:\n{result.ContactLocation.Value.FullName}"
            : $"The directory containing the contacts given by the user does not exist, or the contacts could not be generated.";
        const string nExists = "The directory provided by the user did not exist, so one was generated instead.";

        ((string, string, string), int) returnValues = (directoryExists, result.ContactLocation.IsSuccess, result.UploadedContacts.IsSuccess) switch
        {
            (true, true, true) => ((generated, exists, uploaded), ProgramErrorCodes.Success),
            (true, true, false) => ((generated, exists, nUploaded), ProgramErrorCodes.Contacts_UploadFailed),
            (true, false, true) => ((nGenerated, exists, uploaded), ProgramErrorCodes.Contacts_ContactGenFailed),
            (true, false, false) => ((nGenerated, exists, nUploaded), ProgramErrorCodes.Contacts_ContactsAndUploadFailed),
            (false, true, true) => ((generated, nExists, uploaded), ProgramErrorCodes.Contacts_DirectoryFailed),
            (false, true, false) => ((generated, exists, nUploaded), ProgramErrorCodes.Contacts_DirectoryAndUploadFailed),
            (false, false, true) => ((nGenerated, nExists, uploaded), ProgramErrorCodes.Contacts_DirectoryAndContactsFailed),
            (false, false, false) => ((nGenerated, nExists, nUploaded), ProgramErrorCodes.Contacts_CriticalFailure),
        };
        (string, string, string) strings = returnValues.Item1;
        inform.InformUser(strings.Item1, strings.Item2, strings.Item3);
        return returnValues.Item2;
    }
    #endregion
}