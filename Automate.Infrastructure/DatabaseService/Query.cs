using Automate.Application.InfrastructureValueObjects;
using System.Text.RegularExpressions;

namespace Automate.Infrastructure.DatabaseService;

public partial class Query : IQuery
{
    #region Constructors
    public Query(Query query)
    {
        Type = query.Type;
        Select = query.Select;
        From = query.From;
        Where = query.Where;
        GroupBy = query.GroupBy;
        OrderBy = query.OrderBy;
        QueryString = query.QueryString;
    }
    public Query(DwhQueryType type, string query)
    {
        QueryString = VerifyQuery(query, out string select, out string from, out string? where, out string? groupBy, out string? orderBy);
        Type = type;
        Select = select;
        From = from;
        Where = where;
        GroupBy = groupBy;
        OrderBy = orderBy;
    }
    public Query(DwhQueryType type, string select, string from) : this(type, select, from, null, null, null, true) { }
    public Query(DwhQueryType type, string select, string from, bool addSpaces) : this(type, select, from, null, null, null, addSpaces) { }
    public Query(DwhQueryType type, string select, string from, string? where) : this(type, select, from, where, null, null, true) { }
    public Query(DwhQueryType type, string select, string from, string? where, bool addSpaces) : this(type, select, from, where, null, null, addSpaces) { }
    public Query(DwhQueryType type, string select, string from, string? where, string? groupby) : this(type, select, from, where, groupby, null, true) { }
    public Query(DwhQueryType type, string select, string from, string? where, string? groupby, bool addSpaces) : this(type, select, from, where, groupby, null, addSpaces) { }
    public Query(DwhQueryType type, string select, string from, string? where, string? groupby, string? orderBy) : this(type, select, from, where, groupby, orderBy, true) { }
    public Query(DwhQueryType type, string select, string from, string? where, string? groupby, string? orderby, bool addSpaces)
    {
        Type = type;
        Select = select;
        From = from;
        Where = where;
        GroupBy = groupby;
        OrderBy = orderby;
        QueryString = addSpaces
            ? AddSpaces
            (
                Select,
                From,
                string.IsNullOrWhiteSpace(Where) ? string.Empty : Where,
                string.IsNullOrWhiteSpace(GroupBy) ? string.Empty : GroupBy,
                string.IsNullOrWhiteSpace(OrderBy) ? string.Empty : OrderBy
            )
            : string.Join(string.Empty, QueryStrings);
    }
    #endregion

    #region Public
    public DwhQueryType Type { get; }
    public string Select { get; }
    private const string SelectStr = "SELECT ";
    public string From { get; }
    private const string FromStr = "FROM ";
    public string? Where { get; private set; }
    private const string WhereStr = "WHERE ";
    public string SetWhere(string where) => Set(QueryType.Where, where);
    public string AppendWhere(string where) => Add(QueryType.Where, where);
    public string? GroupBy { get; private set; }
    private const string GroupbyStr = "GROUP BY ";
    public string SetGroupBy(string groupBy) => Set(QueryType.GroupBy, groupBy);
    public string AppendGroupBy(string groupBy) => Add(QueryType.GroupBy, groupBy);
    public string? OrderBy { get; private set; }
    private const string OrderbyStr = "ORDER BY ";
    public string SetOrderBy(string groupBy) => Set(QueryType.OrderBy, groupBy);
    public string AppendOrderBy(string groupBy) => Add(QueryType.OrderBy, groupBy);
    public string QueryString { get; private set; }
    public enum QueryType
    {
        Select,
        From,
        Where,
        GroupBy,
        OrderBy
    }
    #endregion

    #region Private
    private string[] QueryStrings
    {
        get
        {
            // This ensures that if there is no space at the end of each component, one will be added
            // Do not save this value in a field
            List<string> strs =
            [
                Select[^1] == ' ' ? Select : Select + " ",
                From[^1] == ' ' ? From : From + " "
            ];
            if (Where is not null)
                strs.Add(Where[^1] == ' ' ? Where : Where + " ");
            if (GroupBy is not null)
                strs.Add(GroupBy[^1] == ' ' ? GroupBy : GroupBy + " ");
            if (OrderBy is not null)
                strs.Add(OrderBy[^1] == ' ' ? OrderBy : OrderBy + " ");
            return [.. strs];
        }
    }
    private static string AddSpaces(params string?[] args)
    {
        List<string> list = new(args.Length);
        foreach (var arg in args)
        {
            if (!string.IsNullOrWhiteSpace(arg))
                list.Add(arg);
        }
        var result = string.Join(' ', list);
        return result;
    }
    private string Set(QueryType type, string addition)
    {
        switch (type)
        {
            case QueryType.Select: break;
            case QueryType.From: break;
            case QueryType.Where: Where = addition; break;
            case QueryType.GroupBy: GroupBy = addition; break;
            case QueryType.OrderBy: OrderBy = addition; break;
            default: throw new NotImplementedException($"The following sql query keyword has not been implemented: {type}");
        };
        QueryString = string.Join(string.Empty, QueryStrings);
        return QueryString;
    }
    private string Add(QueryType type, string addition)
    {
        switch (type)
        {
            case QueryType.Select: break;
            case QueryType.From: break;
            case QueryType.Where:
                string addIt = WhereRgx().Replace(addition, string.Empty);
                if (Where is null)
                {
                    Where = WhereStr + addIt;
                }
                else
                {
                    // Check whether the addition has the appropriate syntax
                    string[] addArray = addIt.Split(" ").Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                    bool hasand = string.Equals(addArray[0], _and, StringComparison.InvariantCultureIgnoreCase);
                    if (hasand)
                    {
                        if (Where[^1] == ' ')
                            Where = Where + addIt;
                        else
                            Where = Where + " " + addIt;
                    }
                    else
                    {
                        if (Where[^1] == ' ')
                            Where = Where + _and + " " + addIt;
                        else
                            Where = Where + " " + _and + " " + addIt;
                    }
                }
                break;
            case QueryType.GroupBy:
                string readdition = GroupByRgx().Replace(addition, string.Empty);
                GroupBy = GroupBy is null
                    ? GroupbyStr + readdition
                    : GroupBy + ", " + readdition;
                break;
            case QueryType.OrderBy:
                bool ascMatch = AscRgx().IsMatch(addition);
                bool descMatch = DescRgx().IsMatch(addition);
                if (OrderBy is null)
                {
                    if (!descMatch && !ascMatch)
                    {
                        string add = OrderByRgx().Replace(addition, string.Empty);
                        OrderBy = OrderbyStr + add + " asc";
                    }
                    else
                        OrderBy = OrderbyStr + addition;
                }
                else
                {
                    Match asc = AscRgx().Match(OrderBy);
                    Match desc = DescRgx().Match(OrderBy);

                    // If asc, then use asc; if desc, then use desc; otherwise, default to asc, even though the default will likely never be used.
                    string keyword = asc.Success ? asc.Value : desc.Success ? desc.Value : " asc";
                    string inter = AscRgx().Replace(OrderBy, string.Empty);
                    string intermed = DescRgx().Replace(inter, string.Empty);

                    OrderBy = intermed + ", " + addition + " " + keyword;
                }
                break;
            default: throw new NotImplementedException($"The following sql query keyword has not been implemented: {type}");
        }

        var intermediate = VerifyQuery(string.Join(string.Empty, QueryStrings), out _, out _, out _, out _, out _);
        QueryString = string.Join(' ', intermediate.Split(' ').Where(i => !string.IsNullOrWhiteSpace(i)));
        return QueryString;
    }

    private const string _culture = "en-US";
    private const string _and = "AND";
    [GeneratedRegex(_and, RegexOptions.IgnoreCase, _culture)]
    private static partial Regex And();
    [GeneratedRegex(@"asc(ending)?\b", RegexOptions.IgnoreCase, _culture)]
    private static partial Regex AscRgx();
    [GeneratedRegex(@"desc(ending)?\b", RegexOptions.IgnoreCase, _culture)]
    private static partial Regex DescRgx();
    [GeneratedRegex(@"order\s*by ", RegexOptions.IgnoreCase, _culture)]
    private static partial Regex OrderByRgx();
    [GeneratedRegex(@"group\s*by ", RegexOptions.IgnoreCase, _culture)]
    private static partial Regex GroupByRgx();
    [GeneratedRegex(WhereStr, RegexOptions.IgnoreCase, _culture)]
    private static partial Regex WhereRgx();
    [GeneratedRegex("from ", RegexOptions.IgnoreCase, _culture)]
    private static partial Regex FromRgx();
    [GeneratedRegex(@"\bselect ", RegexOptions.IgnoreCase, _culture)]
    private static partial Regex SelectRgx();
    [GeneratedRegex(@"if\(.*,.*,.*\)")]
    private static partial Regex IfRgx();
    #endregion

    #region Internal
    // All internal elements are internal for testing purposes. Please do not refert to private
    internal const string err = "The given query does not contain a properly formatted";
    internal const string err1 = "clause and therefore cannot be executed";
    internal static string VerifyQuery(string query, out string select, out string from, out string? where, out string? groupBy, out string? orderBy)
    {
        orderBy = OrderByIt(query, out string[] orderby);

        groupBy = GroupByIt(query, orderby, out string[] groupby);

        where = WhereIt(query, groupby, out string[] wherer);

        from = FromIt(query, wherer, out string[] fromr);

        select = SelectIt(query, fromr);

        string result = AddSpaces(select, from, where, groupBy, orderBy);

        return result;

        // Locals
        static string? OrderByIt(string query, out string[] orderby)
        {
            string? orderBy;
            orderby = OrderByRgx().Split(query);
            int orderByCt = OrderByRgx().Count(query);
            if (orderByCt > 0)
            {
                int asc = AscRgx().Count(orderby[1]);
                int desc = DescRgx().Count(orderby[1]);
                int ascError = AscRgx().Count(orderby[0]) > 1 ? throw new ArgumentException(Error(QueryType.OrderBy, query, orderby[0], "The asc keyword occurs in the wrong place.")) : 0;
                int descError = DescRgx().Count(orderby[0]) > 1 ? throw new ArgumentException(Error(QueryType.OrderBy, query, orderby[0], "The desc keywords occurs in the wrong place.")) : 0;
                string? intermediary = OrderByRgx().Count(query) == 1 ? "ORDER BY " + orderby[1] : null; // The OrderBy property is nullable because not all queries need one

                if (orderByCt > 0 && asc == 0 && desc == 0) // There is at least one instance of the ORDER BY keyword but no instances of the asc or desc keywords
                {
                    // Add a default
                    if (intermediary is not null)
                        intermediary += " asc";
                }
                else if (orderByCt == 0 && (asc > 0 || desc > 0)) // There are no instances of the ORDER BY keyword but there is at least one asc or desc keyword
                {
                    throw new ArgumentException(Error(QueryType.OrderBy, query, orderby[1], "It has no instances of the \"Order By\" keyword, but there is at least one asc or desc keyword"));
                }
                else if (asc > 0 && desc > 0 || (asc > 1 || desc > 1)) // There is an instance of both asc and desc keywords, but there should only be asc OR desc // Either asc or desc occurs more than once
                {
                    // Set to default
                    if (intermediary is not null)
                    {
                        string inter = AscRgx().Replace(intermediary, string.Empty);
                        intermediary = DescRgx().Replace(inter, string.Empty);
                    }
                }
                orderBy = intermediary;
                //   orderBy = (orderByCt > 0 && asc == 0 && desc == 0) // There is at least one instance of the ORDER BY keyword but no instances of the asc or desc keywords
                //   || (orderByCt == 0 && asc > 0) || (orderByCt == 0 && desc > 0) // There are no instances of the ORDER BY keyword but there is at least one asc or desc keyword
                //   || (asc > 0 && desc > 0) // There is an instance of both asc and desc keywords, but there should only be asc OR desc
                //   || asc > 1 || desc > 1 // Either asc or desc occurs more than once
                //? throw new ArgumentException(Error(QueryType.OrderBy, query, OrderbyStr[1], "It's missing the asc or desc keyword, or there is more than one ORDER BY keyword in the query, or there are too many asc/desc keywords"))
                //: OrderByRgx().Count(query) == 1 ? "ORDER BY " + OrderbyStr[1] : null; // The OrderBy property is nullable because not all queries need one
            }
            else if (AscRgx().Count(query) > 0 || DescRgx().Count(query) > 0) // If we've gotten to this line, there are no ORDER BY keywords in the query
                throw new ArgumentException(Error(QueryType.OrderBy, query, query, "The asc or desc keywords appears in a query without an ORDER BY clause"));
            else orderBy = null; // If we've gotten to this line, there are no ORDER BY keywords in the query
            return orderBy;
        }

        static string? GroupByIt(string query, string[] orderby, out string[] groupby)
        {
            groupby = GroupByRgx().Split(orderby[0]);
            string[] splitgroupby = GroupByRgx().Count(query) == 0 ? groupby[0].Split(',') : groupby[1].Split(','); // This string[] is set to the GROUP BY section split by commas, to ensure that there is not a faulty comma in the GROUP BY clause
            string? groupBy = splitgroupby.Where(s => string.IsNullOrWhiteSpace(s)).ToList().Count > 0 || GroupByRgx().Count(query) > 1
                ? throw new ArgumentException(Error(QueryType.GroupBy, query, groupby[1], "It has one too many commas OR it has more than one GROUP BY clause."))
                : GroupByRgx().Count(query) > 0 ? "GROUP BY " + groupby[1] : null; // The GROUP BY string is nullable because not all queries need a GROUP BY
            return groupBy;
        }

        static string? WhereIt(string query, string[] groupby, out string[] wherer)
        {
            string? where;
            wherer = WhereRgx().Split(groupby[0]);
            int whereCt = WhereRgx().Count(query);
            bool noWhereClause = whereCt == 0; // The query does not contain a WHERE clause
            string where1 = noWhereClause ? string.Empty : wherer[1]; // There may or may not be a WHERE clause
            bool containComma = where1.Contains(',') && !IfRgx().IsMatch(where1); // The where clause should not contain commas unless it has an if expression
            bool q = noWhereClause && // Even though there is no WHERE clause ...
                (where1.Contains('>') || where1.Contains('<') || where1.Contains("'<'")); // ... the query contains greater than or less than operators
            string whery = containComma || whereCt > 1 || q
                ? throw new ArgumentException(Error(QueryType.Where, query, noWhereClause ? wherer[0] : wherer[1], "It has commas in the WHERE clause, or there is more than one WHERE clause, or the WHERE clause is missing when it should not be missing."))
                : noWhereClause ? wherer[0] : wherer[1];
            where = noWhereClause ? null : "WHERE " + whery; // The WHERE string is nullable because not all queries need a WHERE statement
            return where;
        }

        static string FromIt(string query, string[] wherer, out string[] fromr)
        {
            fromr = FromRgx().Split(wherer[0]);
            string[] fromer = FromRgx().Count(query) == 0 ? throw new ArgumentException(Error(QueryType.From, query, fromr[0], "The query does not contain a FROM clause.")) : fromr;
            // This checks whether the from string contains a join clause. If it does, it checks whether it obeys the MySQL syntax formation of a left/right join clause 
            // eg: FROM place LEFT JOIN otherplace ON otherplace.thing = place.thing
            string from1 = fromer[1].Contains(" join ", StringComparison.CurrentCultureIgnoreCase) && !fromer[1].Contains(" on ", StringComparison.CurrentCultureIgnoreCase) && !fromer[1].Contains('=')
                ? throw new ArgumentException(Error(QueryType.GroupBy, query, fromer[1], "It's missing parts of the join clause."))
                : fromer[1];
            string from = FromRgx().Count(query) == 0
                ? throw new ArgumentException(Error(QueryType.GroupBy, query, from1, null))
                : "FROM " + from1;
            return from;
        }

        static string SelectIt(string query, string[] fromr)
        {
            string[] selectr = SelectRgx().Split(fromr[0]); // Split the remainder of the string by the SELECT keyword
            string[] selecter = SelectRgx().Count(query) == 0 ? throw new ArgumentException(Error(QueryType.Select, query, selectr[0], "The query does not contain a SELECT clause.")) : selectr;
            string[] selectColumns = selecter[1].Split(','); // Split columns by commas for review
            string select1 = selectColumns.Where(c => string.IsNullOrWhiteSpace(c)).ToList().Count > 0 // This takes each column, and if even one is empty, then that means we have too many commas in the SELECT clause
                ? throw new ArgumentException(Error(QueryType.Select, query, selecter[1], "The clause has too many commas"))
                : selecter[1];
            string select = string.IsNullOrWhiteSpace(select1) // No columns are named in the SELECT clause
                || SelectRgx().Count(query) == 0 // There is no SELECT keyword
                ? throw new ArgumentException(Error(QueryType.Select, query, select1, "The clause does not contain a SELECT keyword, or there are no columns named"))
                : "SELECT " + select1;
            return select;
        }
    }
    internal static string Error(QueryType type, string query, string badQuerySection, string? error = null)
    {
        string errstr = error is not null ? error : string.Empty;
        return $"{err} {QueryTypeString(type)} {err1}. {errstr}\n\nBAD SECTION:\n{QueryTypeString(type)} {badQuerySection}\nQUERY:\n{query}";
    }
    internal static string QueryTypeString(QueryType type)
    => type switch
    {
        QueryType.Select => "SELECT",
        QueryType.From => "FROM",
        QueryType.Where => "WHERE",
        QueryType.GroupBy => "GROUP BY",
        QueryType.OrderBy => "ORDER BY",
        _ => throw new NotImplementedException($"The given {nameof(QueryType)} string has not been created for this SQL keyword: {type}")
    };
    #endregion
}