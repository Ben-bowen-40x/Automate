using Automate.Cli.Verbs.VerbHelper;
using CommandLine;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "This is an example.")]
internal class ExampleVerb : IVerb
{
    private const string VerbName = "example";

    #region Options
    [Option('f', "fileName", Required = false, HelpText = "Enter the name of the file.")]
    public string FileLocation { get; set; } = string.Empty;
    [Option('s', "secondFile", Required = false, HelpText = "Enter the name of the second file.")]
    public string SecondFileLocation { get; set; } = string.Empty;
    [Option('t', "thirdFile", Required = false, HelpText = "Enter the name of the third file.")]
    public string ThirdFileLocation { get; set; } = string.Empty;
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        static string nothing(string input) =>
            $"The user either chose to provide nothing or the provided path does not exist, so an empty string was used. Here is the literal input: \n\t\"{input}\"";
        string fileLocation =
            FileLocation == string.Empty || !Path.Exists(FileLocation)
            ? nothing(FileLocation)
            : PathManipulation.LocationInformation(FileLocation);
        string secondLocation =
            SecondFileLocation == string.Empty || !Path.Exists(SecondFileLocation)
            ? nothing(SecondFileLocation)
            : PathManipulation.LocationInformation(SecondFileLocation);
        string thirdLocation =
            ThirdFileLocation == string.Empty || !Path.Exists(ThirdFileLocation)
            ? nothing(ThirdFileLocation)
            : PathManipulation.LocationInformation(ThirdFileLocation);

        Console.WriteLine("The following are the options the user chose:");
        Console.WriteLine($"- {nameof(FileLocation)}: \n{fileLocation}");
        Console.WriteLine($"\n- {nameof(SecondFileLocation)}: \n{secondLocation}");
        Console.WriteLine($"\n- {nameof(ThirdFileLocation)}: \n{thirdLocation}");

        int code = ProgramErrorCodes.Success;
        Environment.ExitCode = code;
        return code;
    }
    #endregion
}