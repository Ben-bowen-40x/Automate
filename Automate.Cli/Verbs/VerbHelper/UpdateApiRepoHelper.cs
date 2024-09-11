namespace Automate.Cli.Verbs.VerbHelper;

public enum ApiType
{
    Leaf,
}

internal enum RepoFileTypes
{
    Csv,
    Json,
    Default
}

internal class UpdateApiRepoHelper
{
    public const string ApiTypeHelpText = "Enter the name of the Api you would like to use to update the repo. Your options are: Leaf.";

}
