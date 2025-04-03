using Automate.Application.JsonCsvConversion;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;
using CommandLine;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(_name, HelpText = "Converts a json file into a csv file.")]
internal class ConvertJsonToCsv : IVerb
{
    private const string _name = "convertJsonToCsv";

    #region Options
    [Option('t', "fileType", Required = true, HelpText = JsonConversionHelper.HelpText)]
    public JsonFileType Type { get; set; }
    [Option('j', "jsonFile", Required = true, HelpText = "Enter the name of the json file to be converted to csv file.")]
    public string FileLocation { get; set; } = string.Empty;
    [Option('c', "csvFile", Required = true, HelpText = "Enter the file name where you want the result to be placed.")]
    public string Destination { get; set; } = string.Empty;
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        #region Validate
        // Validate the input
        string parent = PathManipulation.RetrieveParentDir(Destination);
        if (Directory.Exists(parent))
            File.WriteAllText(Destination, "");
        else
        {
            Directory.CreateDirectory(parent);
            File.WriteAllText(Destination, "");
        }

        // Report to the user what happened
        string fileLocation =
            FileLocation == string.Empty || !Path.Exists(FileLocation)
            ? nothing(FileLocation)
            : $"\n- Here is the literal path: \n\t{Path.GetFullPath(FileLocation)}\n- And here is the literal input: \n\t{FileLocation}";
        string thirdLocation =
            Destination == string.Empty || !Path.Exists(parent)
            ? nothing(Destination)
            : $"\n- Here is the literal path: \n\t{Path.GetFullPath(Destination)}\n- And here is the literal input: \n\t{Destination}";
        var inform = service.GetRequiredService<IUserInformation>();
        inform.InformUser("The following are the options the user chose:", $"{nameof(FileLocation)}: {fileLocation}", $"{nameof(Destination)}: {thirdLocation}");

        // The user must provide an existing file in order to convert the file, so if the file doesn't exist, return error
        FileInfo jsonFileLocation;
        if (!File.Exists(FileLocation))
            return ProgramErrorCodes.Error;
        else
            jsonFileLocation = new(FileLocation);

        // By now, the destination has been validated
        FileInfo csvDestination = new(Destination);
        #endregion

        // Execute
        IJsonToCsvConversionManager manager = service.GetRequiredService<IJsonToCsvConversionManager>();
        Result<FileInfo> result = JsonConversionHelper.Execute(Type, manager, jsonFileLocation, csvDestination);

        // Determine error code to return
        int code = DetermineReturnCode(result, inform);
        Environment.ExitCode = code;
        return code;

        // Local
        static string nothing(string input) => $"The user either chose to provide nothing or the provided path does not exist, so an empty string was used. Here is the literal input: {input}";
    }
    #endregion

    #region Private
    private static int DetermineReturnCode<T>(Result<T> result, IUserInformation inform)
    {
        if (result.IsSuccess)
        {
            inform.InformUser($"The program was successful! Here is the result: {result.Value}");
            return ProgramErrorCodes.Success;
        }
        else
        {
            inform.InformUser($"The following error occurred: {result.Error}");
            return ProgramErrorCodes.Error;
        }
    }
    #endregion
}