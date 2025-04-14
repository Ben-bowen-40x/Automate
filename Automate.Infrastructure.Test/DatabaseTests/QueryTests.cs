using Automate.Infrastructure.DatabaseService;

namespace Automate.Infrastructure.Test.DatabaseTests;

public class QueryTests
{
    #region QueryClassProperlyParsesQueryStrings
    [
        Theory,
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc"),
        InlineData("select stuff, things, otherstuff, otherthings place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc"), // This should throw because it's missing the from keyword
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings"), // This should throw because it's missing an on keyword in the left join clause
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings"), // This should throw because it's missing an asc or desc keyword
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings, order by otherthings desc"), // This should throw because of the extra comma in the group by clause
        InlineData("stuff, things, otherstuff, otherthings from place left join otherplace place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings desc"), // This should throw because it doesn't have a SELECT keyword
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace place.one = otherplace.one stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings desc"), // This should throw because it doesn't have a WHERE keyword
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 stuffandthings, thingsandstuff, andstuffthings order by otherthings desc"), // This should throw because it doesn't have a GROUP BY keyword
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings otherthings desc"), // This should throw because it's missing the ORDER BY keyword
    ]
    public void QueryClassProperlyParsesQueryStrings(string query)
    {
        try
        {
            Query q = new(query);
            Assert.NotNull(q);
            Assert.Contains("FROM", q.From);
            Assert.Contains("SELECT", q.Select);

            Query qfromobj = new(q);
            Assert.NotNull(qfromobj);
            Assert.Contains("FROM", qfromobj.From);
            Assert.Contains("SELECT", qfromobj.Select);

        }
        catch (Exception ex)
        {
            Assert.Contains(Query.err, ex.Message); // Asserts that part of the generated error message is contained in the exception message
            Assert.Contains(query, ex.Message); // Asserts that the query is contained in the exception message
        }
    }
    #endregion

    #region QueryClass_Setters_ProperlyAddComponents
    [
    Theory,
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc", "where stuffandthings > 123456789 and otherstuffandthings < 123456789", Query.QueryType.Where, "select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc"),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 order by otherthings asc", "group by stuffandthings, thingsandstuff, andstuffthings", Query.QueryType.GroupBy, "select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc"),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings", "order by otherthings", Query.QueryType.OrderBy, "select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings"),
    ]
    public void QueryClass_Setters_ProperlyAddComponents(string query, string addition, Query.QueryType type, string expected)
    {
        Query q = new(query);
        switch (type)
        {
            case Query.QueryType.Where:
                q.SetWhere(addition);
                break;
            case Query.QueryType.GroupBy:
                q.SetGroupBy(addition);
                break;
            case Query.QueryType.OrderBy:
                q.SetOrderBy(addition);
                break;
            default:
                throw new Exception();
        }
        Assert.Contains(addition.ToLower(), q.QueryString.ToLower());
        Assert.Equal(expected.ToLower() + " ", q.QueryString.ToLower());
    }
    #endregion

    #region QueryClass_Setters_ProperlyAppendComponents
    [
    Theory,
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc", "where stuffandthings > 123456789 and otherstuffandthings < 123456789", Query.QueryType.Where),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where otherthings is not null group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc", "where stuffandthings > 123456789 and otherstuffandthings < 123456789", Query.QueryType.Where),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where otherthings is not null group by stuffandthings, thingsandstuff, andstuffthings order by otherthings asc", "and stuffandthings > 123456789 and otherstuffandthings < 123456789", Query.QueryType.Where),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 order by otherthings asc", "group by stuffandthings, thingsandstuff, andstuffthings", Query.QueryType.GroupBy),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings", "order by otherthings", Query.QueryType.OrderBy),
        InlineData("select stuff, things, otherstuff, otherthings from place left join otherplace on place.one = otherplace.one where stuffandthings > 123456789 and otherstuffandthings < 123456789 group by stuffandthings, thingsandstuff, andstuffthings order by otherthings", "theseotherthings, thisstuff", Query.QueryType.OrderBy),
    ]
    public void QueryClass_Setters_ProperlyAppendComponents(string query, string addition, Query.QueryType type)
    {
        Query q = new(query);
        switch (type)
        {
            case Query.QueryType.Where:
                q.AppendWhere(addition);
                break;
            case Query.QueryType.GroupBy:
                q.AppendGroupBy(addition);
                break;
            case Query.QueryType.OrderBy:
                q.AppendOrderBy(addition);
                break;
            default:
                throw new Exception();
        }
        if (!q.QueryString.Contains(addition, StringComparison.CurrentCultureIgnoreCase))
        {
            var splittee = addition.Split(' ').ToList();
            var split = splittee.Where(i => !string.IsNullOrWhiteSpace(i) && splittee.IndexOf(i) != 0);
            var joined = string.Join(' ', split);
            Assert.Contains(joined.ToLower(), q.QueryString.ToLower());
        }
        else
            Assert.Contains(addition.ToLower(), q.QueryString.ToLower());
    }
    #endregion
}
