using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Automate.Application.UpdateContacts;
using Automate.Domain.SolutionFunctionality;
using Automate.Cli.Verbs.VerbHelper;

namespace Automate.Cli.Verbs;

[Verb(UpdateContacts, HelpText = "Choose to update the contacts list. This either produces or accepts an existing folder location where the contact list files will be housed.")]
internal class ContactUpdateVerb : IVerb
{
    #region Options
    [Option('r', "report", Required = false, HelpText = "Enter the full name of the report file, from C\\ to the folder name where the report set should be deposited. Keep in mind that it must be a directory.")]
    public string ReportDirectory { get; set; } = string.Empty;
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        // Inform the user what's going on
        Console.WriteLine($"For the following option, \"{nameof(ReportDirectory)}\" -- {DirectoryManipulation.LocationInformation(ReportDirectory)}");

        // Validate the user's information
        bool directoryNull = ReportDirectory != string.Empty;
        bool directoryExists = Directory.Exists(ReportDirectory);
        if (directoryExists && !directoryNull)
            Directory.CreateDirectory(ReportDirectory);

        // Execute
        var manager = service.GetRequiredService<IContactUpdateManager>();
        UpdateResult result = manager.UpdateContacts(ReportDirectory);

        // Log
        StringLogger.NameLog(DateTime.Now, UpdateContacts);

        return DetermineReturnCode(result, directoryExists);
    }
    #endregion

    #region Private
    private const string UpdateContacts = "updateContacts";

    private static int DetermineReturnCode(UpdateResult result, bool directoryExists)
    {
        const string generated = "The contacts where generated.";
        const string notGenerated = "At least one contact was not generated";
        string exists = $"The directory containing the contacts given by the user exists. Here is the directory:\n{result.ContactLocation.FullName}";
        string nExists = $"The directory provided by the user did not exist, so one was generated instead. Here is the directory:\n{result.ContactLocation.FullName}";
        const string uploaded = "The contacts were successfully uploaded";
        const string nUploaded = "The contacts were not successfully uploaded";
        if (directoryExists && result.GeneratedContacts && result.UploadedContacts)
        {
            System.Console.WriteLine(generated);
            System.Console.WriteLine(exists);
            System.Console.WriteLine(uploaded);
            return ProgramErrorCodes.Success;
        }
        else if (!directoryExists && result.GeneratedContacts && result.UploadedContacts)
        {
            System.Console.WriteLine(generated);
            System.Console.WriteLine(nExists);
            System.Console.WriteLine(uploaded);
            return ProgramErrorCodes.Contacts_DirectoryFailed;
        }
        else if (directoryExists && !result.GeneratedContacts && result.UploadedContacts)
        {
            System.Console.WriteLine(notGenerated);
            System.Console.WriteLine(exists);
            System.Console.WriteLine(uploaded);
            return ProgramErrorCodes.Contacts_ContactGenFailed;
        }
        else if (directoryExists && result.GeneratedContacts && !result.UploadedContacts)
        {
            System.Console.WriteLine(generated);
            System.Console.WriteLine(exists);
            System.Console.WriteLine(nUploaded);
            return ProgramErrorCodes.Contacts_UploadFailed;
        }
        else if (!directoryExists && !result.GeneratedContacts && result.UploadedContacts)
        {
            System.Console.WriteLine(notGenerated);
            System.Console.WriteLine(nExists);
            System.Console.WriteLine(uploaded);
            return ProgramErrorCodes.Contacts_DirectoryAndContactsFailed;
        }
        else if (!directoryExists && result.GeneratedContacts && !result.UploadedContacts)
        {
            System.Console.WriteLine(generated);
            System.Console.WriteLine(exists);
            System.Console.WriteLine(nUploaded);
            return ProgramErrorCodes.Contacts_DirectoryAndUploadFailed;
        }
        else if (directoryExists && !result.GeneratedContacts && !result.UploadedContacts)
        {
            System.Console.WriteLine(notGenerated);
            System.Console.WriteLine(exists);
            System.Console.WriteLine(nUploaded);
            return ProgramErrorCodes.Contacts_ContactsAndUploadFailed;
        }
        else if (!directoryExists && !result.GeneratedContacts && !result.UploadedContacts)
        {
            System.Console.WriteLine(notGenerated);
            System.Console.WriteLine(nExists);
            System.Console.WriteLine(nUploaded);
            return ProgramErrorCodes.Contacts_CriticalFailure;
        }
        System.Console.WriteLine("An unknown error occurred");
        return ProgramErrorCodes.Contacts_Unknown;
    }
    #endregion
}