namespace Automate.Cli.Verbs.VerbHelper;

public enum RepoType
{
    Leaf,
    Calls,
    Customers,
    ContactForms,
    ContactUpdate,
    Discrepancy,
}

internal class UpdateRepoHelper
{
    public const string RepoTypeHelpText = """
        Enter the name of the repo you would like to update. Your options are: 
        Leaf,
        Calls,
        Customers,
        ContactForms,
        ContactUpdate,
        Discrepancy,
        """;

}
