using Automate.Application.InfrastructureValueObjects;

namespace Automate.Infrastructure.DatabaseService;
public class RawQuery(IRawQuerySettings settings)
{
    readonly IRawQuerySettings _s = settings;

    #region Basic Queries
    // Public Basic
    /// <summary>
    /// Filters the given <paramref name="query"/> based on its <paramref name="type"/> using <see cref="long"/> <paramref name="values"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="query"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public IQuery Filter(DwhQueryType type, IQuery query, List<long> values)
    {
        string vals = string.Join(",", values);
        string where = type switch
        {
            DwhQueryType.AllCalls => $"{_s.CallBasicNumerical!} in ({vals})",
            DwhQueryType.AllCustomers => $"{_s.CustomerBasicNumerical!} in ({vals})",
            _ => throw new Exception($"The type of query has not been set\nType: {type}\nQuery:\n{query}")
        };
        query.AppendWhere(where);
        return query;
    }
    public IQuery CallBasicAddon => new Query(DwhQueryType.AllCalls, _s.CallBasic! + _s.CallBasicAddon);
    public IQuery CustomerBasic => new Query(DwhQueryType.AllCustomers, _s.CustomerBasic!);

    #endregion

    #region Message Queries

    // Public Message Query Members
    /// <summary>
    /// When the day and year are the same and the customer date is after the subscription date, then the sub date is off for some reason in the original data
    /// </summary>
    public string MessageCustomerQuery(List<long> numbers)
    {
        string nums = string.Join(',', numbers);
        string result = $"{CustomerBasic.QueryString} {_s.MessageCustQuery1!} ({nums}) {_s.MessageCustQuery2!};";
        return result;
    }

    /// <summary>
    /// When the day and year are the same and the customer date is after the subscription date, then the sub date is off for some reason in the original data
    /// </summary>
    public string MessageCustomerQuery()
    {
        return CustomerBasic.QueryString;
    }

    public readonly TimeSpan NinetyDays = TimeSpan.FromDays(90);

    /// <summary>
    /// Accepts a <see cref="DateTimeOffset"/> <paramref name="startDate"/> 
    /// </summary>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public string MessageCallQuery(DateTimeOffset startDate, List<long> numbers)
    {
        var threeMonths = startDate - NinetyDays;
        var date = threeMonths.Date.ToString(_s.QueryDateFormat!);
        string nums = string.Join(',', numbers);
        string query = $"{_s.CallBasic! + _s.MessageCallQuery1!} '{date}' {_s.MessageCallQuery2!} {_s.MessageCallQuery3!} ({nums});";
        return query;
    }

    /// <summary>
    /// Accepts a <see cref="DateTimeOffset"/> <paramref name="startDate"/> 
    /// </summary>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public IQuery MessageCallQuery(DateTimeOffset startDate)
    {
        var threeMonths = startDate - NinetyDays;
        var date = threeMonths.Date.ToString(_s.QueryDateFormat!);
        string str = $"{_s.CallBasic! + _s.MessageCallQuery1!} '{date}' {_s.MessageCallQuery2!};";
        IQuery query = new Query(DwhQueryType.MessageCall, str);
        return query;
    }
    #endregion

    #region Discrepancy Query
    /// <summary>
    /// Returns the Discrepancy Query as a raw string
    /// </summary>
    /// <returns>
    /// <para><see cref="string"/> that is the raw sql query</para>
    /// </returns>
    public IQuery DiscrepancyQuery()
    {
        return DiscrepancyQuery(new DateTime(2023, 10, 1));
    }

    /// <summary>
    /// Accepts a <see cref="DateTime"/> <paramref name="start"/>, which defines when the query should pull records, and <paramref name="end"/>, which is the most recent date
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public IQuery DiscrepancyQuery(DateTime start, DateTime end)
    {
        string startString = start.ToString(_s.QueryDateFormat!);
        string endString = end.ToString(_s.QueryDateFormat!);
        string str = $"{_s.Discrepancy!} '{startString}' AND '{endString}' {_s.Discrepancy2!}"; // Keep this here for debugging purposes
        IQuery result = new Query(DwhQueryType.Discrepancy, str);
        return result;
    }

    /// <summary>
    /// Accepts a <see cref="DateTime"/>, <paramref name="start"/>, which defines when the query should pull records
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public IQuery DiscrepancyQuery(DateTime start)
    {
        return DiscrepancyQuery(start, DateTime.Now);
    }
    #endregion

    #region Contact Update Query
    // Public Contact Query Members
    /// <summary>
    /// <para>Uses a <paramref name="number"/>, which is a 10-digit phone number represented as a <see cref="uint"/></para>
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public string ContactQuery(uint number)
    {
        ulong num = _s.ContactUpdateNumber * number;
        ulong num2 = _s.ContactUpdateNumber + num;
        string result = _s.ContactUpdate1! + $" {num} " + _s.ContactUpdate2! + $" {num2} " + _s.ContactUpdate3!;
        return result;
    }

    #endregion

    #region Web Forms Query
    public IQuery WebFormQuery1 => new Query(DwhQueryType.ContactForms, _s.WebFormQuery1!);
    public string WebFormQuery2 => _s.WebFormQuery2!;
    #endregion
}
