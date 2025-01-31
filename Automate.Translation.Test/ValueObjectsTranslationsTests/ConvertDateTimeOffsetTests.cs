using Automate.Translation.ValueObjectTranslate;

namespace Automate.Translation.Test.ValueObjectsTranslationsTests;

public class ConvertDateTimeOffsetTests
{
    #region ConvertLocalToDTOffset(DateTime localTime, TimeZoneEnum zone)
    [
        Theory,
        // year, month, day, hour, minute, second, offset(hours)
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -4 }, TimeZoneEnum.Eastern),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -5 }, TimeZoneEnum.Eastern),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -5 }, TimeZoneEnum.Central),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -6 }, TimeZoneEnum.Central),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -6 }, TimeZoneEnum.Mountain),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -7 }, TimeZoneEnum.Mountain),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -7 }, TimeZoneEnum.Pacific),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -8 }, TimeZoneEnum.Pacific),
    ]
    public void ConvertDateTimeAndTimeZoneEnum(int[] dateInt, TimeZoneEnum zone)
    {
        DateTime newDate = MakeDateFromIntArray(dateInt[0], dateInt[1], dateInt[2], dateInt[3], dateInt[4], dateInt[5]);
        DateTimeOffset expected = GetExpectedDateTimeOffset(dateInt[6], newDate);
        var actual = ConvertDateTimeOffset.ConvertLocalToDTOffset(newDate, zone);
        Assert.Equal(expected, actual);
    }

    #endregion

    #region ConvertLocalToDTOffset(DateTime localTime, TimeZoneEnum zone, out DateTimeOffset result)
    [
        Theory,
        // year, month, day, hour, minute, second, offset(hours)
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -4 }, TimeZoneEnum.Eastern, true),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -5 }, TimeZoneEnum.Eastern, true),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -5 }, TimeZoneEnum.Central, true),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -6 }, TimeZoneEnum.Central, true),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -6 }, TimeZoneEnum.Mountain, true),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -7 }, TimeZoneEnum.Mountain, true),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -7 }, TimeZoneEnum.Pacific, true),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -8 }, TimeZoneEnum.Pacific, true),
    ]
    public void ConvertDateTimeAndTimeZoneWithOutParameter(int[] dateInt, TimeZoneEnum zone, bool expected)
    {
        DateTime newDate = MakeDateFromIntArray(dateInt[0], dateInt[1], dateInt[2], dateInt[3], dateInt[4], dateInt[5]);
        DateTimeOffset expectedDate = GetExpectedDateTimeOffset(dateInt[6], newDate);
        var actual = ConvertDateTimeOffset.ConvertLocalToDTOffset(newDate, zone, out DateTimeOffset actualDate);
        Assert.Equal(expectedDate, actualDate);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertLocalToDTOffset(DateTime date, TimeSpan offset)
    [
        Theory,
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -4 }),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -5 }),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -5 }),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -6 }),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -6 }),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -7 }),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -7 }),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -8 }),
    ]
    public void ConvertDateTimeAndTimeZoneAsTimeSpan(int[] dateTimeInt)
    {
        DateTime newDate = MakeDateFromIntArray(dateTimeInt[0], dateTimeInt[1], dateTimeInt[2], dateTimeInt[3], dateTimeInt[4], dateTimeInt[5]);
        DateTimeOffset expected = GetExpectedDateTimeOffset(dateTimeInt[6], newDate);
        var actual = ConvertDateTimeOffset.ConvertLocalToDTOffset(newDate, TimeSpan.FromHours(dateTimeInt[6]));
        Assert.Equal(expected, actual);
    }
    #endregion

    /*
    #region ConvertLocalToDTOffset(DateTime date, TimeZoneInfo timeZone)
    [
        Theory,
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -4 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -5 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -5 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -6 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -6 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -7 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -7 },TimeZoneInfo.Utc),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -8 },TimeZoneInfo.Utc),
    ]
    public void ConvertDateTimeAndTimeZoneInfo(int[] dateInt, TimeZoneInfo zone)
    {

    }
    #endregion
    //*/

    #region Private
    internal static DateTimeOffset GetExpectedDateTimeOffset(int dateInt, DateTime newDate)
    {
        return new(newDate - TimeSpan.FromHours(dateInt), TimeSpan.FromHours(0));
    }

    internal static DateTime MakeDateFromIntArray(int dateInt0, int dateInt1, int dateInt2, int dateInt3, int dateInt4, int dateInt5)
    {
        var value = dateInt0 == 0 || dateInt1 == 0 || dateInt2 == 0 || dateInt3 == 0 || dateInt4 == 0 || dateInt5 == 0
            ? DateTime.MinValue
            : new DateTime(dateInt0, dateInt1, dateInt2, dateInt3, dateInt4, dateInt5);

        return value;
    }
    #endregion
}
