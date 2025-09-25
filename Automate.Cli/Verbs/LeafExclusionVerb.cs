using Automate.Application.LeafExclusion;
using Automate.Cli.Verbs.VerbHelper;
using Automate.Domain.SolutionFunctionality;
using CommandLine;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "This is an example.")]
internal class LeafExclusionVerb : IVerb
{
    private const string VerbName = "leafExclusion";

    #region Options
    [Option('f', "fileName", Required = true, HelpText = "Enter the name of the leaf repository file.")]
    public required FileInfo LeafRepoLocation { get; set; }
    [Option('o', "output", Required = true, HelpText = "Enter the name of the file where the exclusion list will reside.")]
    public required FileInfo Output { get; set; }
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        static string nothing(string input) =>
            $"The user either chose to provide nothing or the provided path does not exist, so an empty string was used. Here is the literal input: \n\t\"{input}\"";
        string fileLocation =
            !LeafRepoLocation.Exists
            ? nothing(LeafRepoLocation.FullName)
            : PathManipulation.LocationInformation(LeafRepoLocation.FullName);
        string secondLocation =
            !Output.Exists
            ? nothing(Output.FullName)
            : PathManipulation.LocationInformation(Output.FullName);

        var inform = service.GetRequiredService<IUserInformation>();
        string options = "The following are the options the user chose:";
        string fileLocMsg = $"- {nameof(LeafRepoLocation)}: \n{fileLocation}";
        string secondLocMsg = $"\n- {nameof(Output)}: \n{secondLocation}";
        inform.InformUser(options, fileLocMsg, secondLocMsg);

        ILeafExclusionManager manager = service.GetRequiredService<ILeafExclusionManager>();
        Result result = manager.Manage(LeafRepoLocation, Output);

        int code = result.IsSuccess ? ProgramErrorCodes.Success : ProgramErrorCodes.Error;
        Environment.ExitCode = code;
        return code;
    }
    #endregion
}