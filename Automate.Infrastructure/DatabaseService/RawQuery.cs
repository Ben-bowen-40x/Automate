using Automate.Application.InfrastructureValueObjects;

namespace Automate.Infrastructure.DatabaseService;
public class RawQuery(IRawQuerySettings settings)
{
    readonly IRawQuerySettings _s = settings;
    private string QueryDateFormat => _s.QueryDateFormat!;

    #region Basic Queries
    // Public Basic
    public string Filter(DwhQueryType type, string query, List<long> values)
    {
        string vals = string.Join(",", values);
        return type switch
        {
            DwhQueryType.AllCalls => $"{query} and {CallBasicNumerical} in ({vals})",
            DwhQueryType.AllCustomers => $"{query} and {CustomerBasicNumerical} in ({vals})",
            _ => query
        };
    }
    public string CallBasicAddon => CallBasic + _s.CallBasicAddon;
    public string CustomerBasic => _s.CustomerBasic!;

    // Private Basic
    private string CallBasicNumerical => _s.CallBasicNumerical!;
    private string CustomerBasicNumerical => _s.CustomerBasicNumerical!;
    private string CallBasic => _s.CallBasic!;

    #endregion

    #region Message Queries

    // Public Message Query Members
    /// <summary>
    /// When the day and year are the same and the customer date is after the subscription date, then the sub date is off for some reason in the original data
    /// </summary>
    public string MessageCustomerQuery(List<long> numbers)
    {
        string nums = string.Join(',', numbers);
        return $"{MessageCustSubQuery} {_messageCustSubQuery2} ({nums}) {_messageCustSubQuery3};";
    }

    /// <summary>
    /// When the day and year are the same and the customer date is after the subscription date, then the sub date is off for some reason in the original data
    /// </summary>
    public string MessageCustomerQuery()
    {
        return MessageCustSubQuery;
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
        var date = threeMonths.Date.ToString(QueryDateFormat);
        string nums = string.Join(',', numbers);
        string query = $"{MessageCallQuery1} '{date}' {MessageCallQuery2} {MessageCallQuery3} ({nums});";
        return query;
    }

    /// <summary>
    /// Accepts a <see cref="DateTimeOffset"/> <paramref name="startDate"/> 
    /// </summary>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public string MessageCallQuery(DateTimeOffset startDate)
    {
        var threeMonths = startDate - NinetyDays;
        var date = threeMonths.Date.ToString(QueryDateFormat);
        string query = $"{MessageCallQuery1} '{date}' {MessageCallQuery2};";
        return query;
    }

    // Private Call Query Members
    private string MessageCallQuery1 => CallBasic + _s.MessageCallQuery1!;

    private string MessageCallQuery2 => _s.MessageCallQuery2!;

    private string MessageCallQuery3 => _s.MessageCallQuery3!;

    // Private Customer Query Members
    private string MessageCustSubQuery => CustomerBasic;

    private string _messageCustSubQuery2 => _s.MessageCustQuery2!;
    private string _messageCustSubQuery3 => _s.MessageCustQuery3!;

    #endregion

    #region Discrepancy Query
    // Public Discrepancy Query Members
    /// <summary>
    /// Returns the Discrepancy Query as a raw string
    /// </summary>
    /// <returns>
    /// <para><see cref="string"/> that is the raw sql query</para>
    /// </returns>
    public string DiscrepancyQuery()
    {
        return DiscrepancyQuery(new DateTime(2023, 10, 1));
    }

    /// <summary>
    /// Accepts a <see cref="DateTime"/> <paramref name="start"/>, which defines when the query should pull records, and <paramref name="end"/>, which is the most recent date
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public string DiscrepancyQuery(DateTime start, DateTime end)
    {
        string startString = start.ToString(QueryDateFormat);
        string endString = end.ToString(QueryDateFormat);
        string result = $"{Discrepancy} '{startString}' {and} '{endString}' {Discrepancy2}"; // Keep this here for debugging purposes
        return result;
    }

    /// <summary>
    /// Accepts a <see cref="DateTime"/>, <paramref name="start"/>, which defines when the query should pull records
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public string DiscrepancyQuery(DateTime start)
    {
        return DiscrepancyQuery(start, DateTime.Now);
    }

    // Private Discrepancy Query Members
    private string Discrepancy => _s.Discrepancy!;

    private const string and = "AND";

    private string Discrepancy2 => _s.Discrepancy2!;

    private string OriginalDiscrepancy => _s.OriginalDiscrepancy!;

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
        ulong num = ContactUpdateNumber * number;
        ulong num2 = ContactUpdateNumber + num;
        return ContactUpdate + $" {num} " + ContactUpdate2 + $" {num2} " + ContactUpdate3;
    }

    // Private Contact Query members
    private string ContactUpdate => _s.ContactUpdate1!;

    private string ContactUpdate2 => _s.ContactUpdate2!;
    private string ContactUpdate3 => _s.ContactUpdate3!;
    private ulong ContactUpdateNumber => _s.ContactUpdateNumber;
    #endregion

    #region Web Forms Query
    public string WebFormQuery1 => _s.WebFormQuery1!;
    public string WebFormQuery2 => _s.WebFormQuery2!;
    #endregion
}