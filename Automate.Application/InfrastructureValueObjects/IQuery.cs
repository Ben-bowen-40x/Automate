namespace Automate.Application.InfrastructureValueObjects;

public interface IQuery
{
    string Select { get; }
    string From { get; }
    string? Where { get; }
    string? GroupBy { get; }
    string? OrderBy { get; }
    string QueryString { get; }

    string AppendGroupBy(string groupBy);
    string AppendOrderBy(string groupBy);
    string AppendWhere(string where);
    string SetGroupBy(string groupBy);
    string SetOrderBy(string groupBy);
    string SetWhere(string where);
}