using Automate.Application.DwhRepoUpdate;
using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.SolutionFunctionality;
using CommandLine;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Cli.Verbs;

[Verb(VerbName, HelpText = "Choose a file type, and choose where to output the result, and a query will be made to the dwh to retrieve values based on the query found in the file type. The output will be a csv file")]
internal class UpdateDwhRepoVerb : IVerb
{
    private const string VerbName = "updateDwhRepo";

    #region Options
    [Option('o', "outPutFile", Required = true, HelpText = "Enter the name of the file where the output will go. This must be a csv file.")]
    public required FileInfo ValueRepoLocation { get; set; }
    [Option('t', "sqlFileType", Required = true, HelpText = "Enter the sql file type that we wish to query. Your options are as follows: " + SqlFileTypeHelper.HelpText)]
    public SqlFileType Filetype { get; set; }
    #endregion

    #region Public (Other than Options)
    public int Run(IServiceProvider service)
    {
        IUserInformation inform = service.GetRequiredService<IUserInformation>();
        inform.InformUser("The user chose the following options", $"{nameof(ValueRepoLocation)}:", $"- {ValueRepoLocation.FullName}", $"{nameof(Filetype)}:", $"- {Filetype}", string.Empty);
        if (!ValueRepoLocation.Extension.Equals(".csv"))
        {
            string error = $"Input must be a csv file. This is the input:\n\"{ValueRepoLocation.FullName}\"\n";
            inform.InformUser(error);
            throw new ArgumentException(error);
        }

        IDwhRepoUpdateManager manager = service.GetRequiredService<IDwhRepoUpdateManager>();
        Result result = manager.Manage(Filetype, ValueRepoLocation);

        int code = result.IsSuccess
            ? ProgramErrorCodes.Success
            : ProgramErrorCodes.Error;
        Environment.ExitCode = code;
        return code;
    }
    #endregion
}