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
        DateTime newDate = Functions.IntsToDateTime(dateInt[0], dateInt[1], dateInt[2], dateInt[3], dateInt[4], dateInt[5]);
        DateTimeOffset expected = Functions.GetExpectedDateTimeOffset(dateInt[6], newDate);
        var actual = ConvertDateTimeOffset.Convert(newDate, zone);
        Assert.Equal(expected, actual);
    }

    #endregion

    #region ConvertLocalToDTOffset(DateTime localTime, TimeZoneEnum zone, out DateTimeOffset result)
    [
        Theory,
        // year, month, day, hour, minute, second, offset(hours)
        // Remember that the offsets are specific to Daylight Savings or Standard Time
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -4 }, TimeZoneEnum.Eastern),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -5 }, TimeZoneEnum.Eastern),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -5 }, TimeZoneEnum.Central),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -6 }, TimeZoneEnum.Central),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -6 }, TimeZoneEnum.Mountain),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -7 }, TimeZoneEnum.Mountain),
        InlineData(new int[] { 2024, 07, 14, 15, 07, 33, -7 }, TimeZoneEnum.Pacific),
        InlineData(new int[] { 2024, 01, 14, 15, 07, 33, -8 }, TimeZoneEnum.Pacific),
    ]
    public void ConvertDateTimeAndTimeZoneWithOutParameter(int[] dateInt, TimeZoneEnum zone)
    {
        // Assemble
        DateTime newDate = Functions.IntsToDateTime(dateInt[0], dateInt[1], dateInt[2], dateInt[3], dateInt[4], dateInt[5]);
        DateTimeOffset expectedDate = Functions.GetExpectedDateTimeOffset(dateInt[6], newDate);

        // Act
        bool actual = ConvertDateTimeOffset.TryConvert(newDate, zone, out DateTimeOffset actualDate);

        // Assert
        Assert.Equal(expectedDate, actualDate);
        Assert.True(actual);
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
        DateTime newDate = Functions.IntsToDateTime(dateTimeInt[0], dateTimeInt[1], dateTimeInt[2], dateTimeInt[3], dateTimeInt[4], dateTimeInt[5]);
        DateTimeOffset expected = Functions.GetExpectedDateTimeOffset(dateTimeInt[6], newDate);
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

}
