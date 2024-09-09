using Automate.Application.JsonCsvConversion;
using Automate.Infrastructure.JsonToCsvService.CsvMaps;
using Automate.Infrastructure.JsonToCsvService.JsonMaps;

namespace Automate.Cli.Verbs.VerbHelper;

public enum JsonFileType
{
    DwhContactForms
}
internal class JsonConversionHelper
{
    public const string HelpText = "Enter the type of file that the input file will be. The following is/are your options: DwhContactForms";

    internal static Dictionary<bool, FileInfo> Execute(JsonFileType type, IJsonToCsvConversionManager manager, FileInfo jsonFileLocation, FileInfo csvDestination)
    {
        return type switch
        {
            JsonFileType.DwhContactForms => manager.ManageConversion<JsonMessage, MessageMap>(jsonFileLocation, csvDestination),
            _ => manager.ManageConversion<JsonMessage, MessageMap>(jsonFileLocation, csvDestination)
        };
    }
}
