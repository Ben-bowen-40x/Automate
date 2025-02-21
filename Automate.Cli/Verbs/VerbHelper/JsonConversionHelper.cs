using Automate.Application.JsonCsvConversion;
using Automate.Infrastructure.DataRetrievalFormats;
using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs.VerbHelper;

public enum JsonFileType
{
    DwhContactForms,
}

internal class JsonConversionHelper
{
    public const string HelpText = """
        Enter the type of the input file. This is necessary because the structure of the json file must be known before it can be processed and converted. 
        The following is/are your options: 
        DwhContactForms,
        """;

    internal static Result<FileInfo> Execute(JsonFileType type, IJsonToCsvConversionManager manager, FileInfo jsonFileLocation, FileInfo csvDestination)
    {
        switch (type)
        {
            case JsonFileType.DwhContactForms:
                var result = manager.ManageConversion<JsonMessageMap, JsonMessageMap>(jsonFileLocation, csvDestination);

                // We want to throw here because all executions are generic and therefore don't have full context.
                // Throwing here helps us understand the full context of the error
                if (result.IsFailure) throw new Exception(result.Error);
                return result;
            default:
                throw new ArgumentException($"The user failed to follow the following parameters:\n{HelpText}");
        }
    }
}
