namespace Automate.Application.InfrastructureValueObjects;

public interface IQuery
{
    DwhQueryType Type { get; }
    string Select { get; }
    string From { get; }
    string? Where { get; }
    string? GroupBy { get; }
    string? OrderBy { get; }
    string QueryString { get; }

    string SetGroupBy(string groupBy);
    string SetOrderBy(string groupBy);
    string SetWhere(string where);
}