using Automate.Application.JsonCsvConversion;
using Automate.Cli.Verbs.VerbHelper;
using CommandLine;
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
      static string Nothing(string input) =>
          $"The user either chose to provide nothing or the provided path does not exist, so an empty string was used. Here is the literal input: {input}";
      string fileLocation =
          FileLocation == string.Empty || !Path.Exists(FileLocation)
          ? Nothing(FileLocation)
          : $"\n- Here is the literal path: \n\t{Path.GetFullPath(FileLocation)}\n- And here is the literal input: \n\t{FileLocation}";
      string thirdLocation =
          Destination == string.Empty || !Path.Exists(parent)
          ? Nothing(Destination)
          : $"\n- Here is the literal path: \n\t{Path.GetFullPath(Destination)}\n- And here is the literal input: \n\t{Destination}";
      Console.WriteLine("The following are the options the user chose:");
      Console.WriteLine($"{nameof(FileLocation)}: {fileLocation}");
      Console.WriteLine($"{nameof(Destination)}: {thirdLocation}");

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
      Dictionary<bool, FileInfo> result = JsonConversionHelper.Execute(Type, manager, jsonFileLocation, csvDestination);

      // TODO: Determine error code to return


      return ProgramErrorCodes.Success;
   }
   #endregion

   #region Private

   #endregion
}