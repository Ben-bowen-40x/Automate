using Automate.Application.JsonCsvConversion;
using Automate.Infrastructure.JsonToCsvService.CsvMaps;
using Automate.Infrastructure.JsonToCsvService.JsonMaps;
using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs.VerbHelper;

public enum JsonFileType
{
    DwhContactForms,
}

internal class JsonConversionHelper
{
    public const string HelpText = "Enter the type of file that the input file will be. The following is/are your options: DwhContactForms";

    internal static Result<FileInfo> Execute(JsonFileType type, IJsonToCsvConversionManager manager, FileInfo jsonFileLocation, FileInfo csvDestination)
    {
        return type switch
        {
            // TODO: Translation Layer should handle these translations
            JsonFileType.DwhContactForms => manager.ManageConversion<JsonMessage, MessageMap>(jsonFileLocation, csvDestination),
            _ => manager.ManageConversion<JsonMessage, MessageMap>(jsonFileLocation, csvDestination)
        };
    }
}
