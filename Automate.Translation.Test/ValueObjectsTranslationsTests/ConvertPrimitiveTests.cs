using Automate.Translation.ValueObjectTranslate;

namespace Automate.Translation.Test;

public class ConvertPrimitiveTests
{
    #region ConvertBool(int?)
    [
        Theory,
        InlineData(1, true),
        InlineData(2, true),
        InlineData(0, false),
        InlineData(-1, false),
        InlineData(null, false),
    ]
    public void ConvertNullableIntToBool(int? input, bool expected)
    {
        bool actual = ConvertPrimitive.ConvertBool(input);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertBool(string)
    [
        Theory,
        InlineData("true", true),
        InlineData("billable", true),
        InlineData("billabel", true),
        InlineData("bilable", true),
        InlineData("bilabel", true),
        InlineData("false", false),
        InlineData("non billable", false),
        InlineData("non-billable", false),
        InlineData("non", false),
        InlineData("not billable", false),
        InlineData("not-billable", false),
        InlineData("not", false),
        InlineData(null, false),
    ]
    public void ConvertNullableStrToBool(string? input, bool expected)
    {
        bool actual = ConvertPrimitive.ConvertBool(input);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region VerifyValue(double?)
    [
        Theory,
        InlineData(null, 0.0),
        InlineData(1.23, 1.23),
        InlineData(55.53, 55.53),
    ]
    public void VerifyNullableDoubleToDouble(double? input, double expected)
    {
        double actual = ConvertPrimitive.VerifyValue(input);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertTimeSpan(string?)
    [
        Theory,
        InlineData(null, new int[] { 0, 0, 0 }),
        InlineData("256", new int[] { 0, 0, 256 }),
        InlineData("1:22:04", new int[] { 1, 22, 4 }),
    ]
    public void ConvertNullableStringToTimeSpan_1(string? input, int[] expectedInts)
    {
        TimeSpan expected = new(expectedInts[0], expectedInts[1], expectedInts[2]);
        TimeSpan actual = ConvertPrimitive.ConvertTimeSpan(input);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertTimeSpan(string?) 2
    [
        Theory,
        InlineData("53.26", 53.26),
        InlineData("45.45", 45.45)
    ]
    public void ConvertNullableStringToTimeSpan_2(string? input, double expectedDouble)
    {
        TimeSpan expected = TimeSpan.FromSeconds(expectedDouble);
        TimeSpan actual = ConvertPrimitive.ConvertTimeSpan(input);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertDateTimeOffset(string?, DateTimeDefault)
    [
        Theory,
        InlineData(null, DateDefault.Min, null),
        InlineData(null, DateDefault.Max, null), // Year, month, day, hour, minute, second, offset(hours)
        InlineData("2025-01-06T19:45:03-08:00", DateDefault.Min, new int[] { 2025, 01, 06, 19, 45, 03, -8 }),
        InlineData("2025-01-06T19:45:03-08:00", DateDefault.Max, new int[] { 2025, 01, 06, 19, 45, 03, -8 }),
        InlineData("2024-12-14T07:03:15-07:00", DateDefault.Min, new int[] { 2024, 12, 14, 07, 03, 15, -7 }),
        InlineData("2024-12-14T07:03:15-07:00", DateDefault.Max, new int[] { 2024, 12, 14, 07, 03, 15, -7 }),
        InlineData("2025-01-06 19:45:03-08:00", DateDefault.Min, new int[] { 2025, 01, 06, 19, 45, 03, -8 }),
        InlineData("2025-01-06 19:45:03-08:00", DateDefault.Max, new int[] { 2025, 01, 06, 19, 45, 03, -8 }),
        InlineData("2024-12-14 07:03:15-07:00", DateDefault.Min, new int[] { 2024, 12, 14, 07, 03, 15, -7 }),
        InlineData("2024-12-14 07:03:15-07:00", DateDefault.Max, new int[] { 2024, 12, 14, 07, 03, 15, -7 }),
    ]
    public void ConvertDateStrToDateTimeOffset(string? date, DateDefault def, int[]? expectedInts)
    {
        DateTimeOffset expected = expectedInts is not null
            ? new DateTimeOffset(
                new DateTime(expectedInts[0], expectedInts[1], expectedInts[2], expectedInts[3], expectedInts[4], expectedInts[5]),
                TimeSpan.FromHours(expectedInts[6]))
            : def switch { DateDefault.Max => DateTimeOffset.MaxValue, _ => DateTimeOffset.MinValue };

        DateTimeOffset actual = ConvertPrimitive.ConvertDateTimeOffset(date, def);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertDateTimeOffset(string?, TimeZoneEnum, DateTimeDefault)
    [
        Theory,
        // year, month, day, hour, minute, second, offset(hours)
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -4 }, TimeZoneEnum.Eastern, DateDefault.Min),
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -4 }, TimeZoneEnum.Eastern, DateDefault.Max),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -5 }, TimeZoneEnum.Eastern, DateDefault.Min),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -5 }, TimeZoneEnum.Eastern, DateDefault.Max),
        // year, month, day, hour, minute, second, offset(hours)
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -5 }, TimeZoneEnum.Central, DateDefault.Min),
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -5 }, TimeZoneEnum.Central, DateDefault.Max),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -6 }, TimeZoneEnum.Central, DateDefault.Min),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -6 }, TimeZoneEnum.Central, DateDefault.Max),
        // year, month, day, hour, minute, second, offset(hours)
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -6 }, TimeZoneEnum.Mountain, DateDefault.Min),
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -6 }, TimeZoneEnum.Mountain, DateDefault.Max),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -7 }, TimeZoneEnum.Mountain, DateDefault.Min),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -7 }, TimeZoneEnum.Mountain, DateDefault.Max),
        // year, month, day, hour, minute, second, offset(hours)
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -7 }, TimeZoneEnum.Pacific, DateDefault.Min),
        InlineData(new int[] { 2024, 06, 09, 05, 03, 59, -7 }, TimeZoneEnum.Pacific, DateDefault.Max),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -8 }, TimeZoneEnum.Pacific, DateDefault.Min),
        InlineData(new int[] { 2025, 01, 04, 15, 53, 02, -8 }, TimeZoneEnum.Pacific, DateDefault.Max),
    ]
    public void ConvertNullableStringToDateTimeOffset(int[] input, TimeZoneEnum zone, DateDefault def)
    {
        DateTime newDate = new(input[0], input[1], input[2], input[3], input[4], input[5]);
        DateTimeOffset expected = new(newDate - TimeSpan.FromHours(input[6]), TimeSpan.FromSeconds(0));
        DateTimeOffset actual = ConvertPrimitive.ConvertDateTimeOffset(newDate, zone, def);
        Assert.Equal(expected, actual);
    }
    #endregion

    #region ConvertDate(string? date, string? time, DateTimeDefault)
    [
        Theory,
        InlineData(null, null, DateDefault.Min, false),
        InlineData(null, "6:35 PM utc", DateDefault.Max, false),
        InlineData(null, "6:35 PM utc", DateDefault.Min, false),
        InlineData("February 3, 2025", null, DateDefault.Min, true),
        InlineData("February 3, 2025", null, DateDefault.Max, true),
        InlineData("2024-02-03", "15:07:12 UTC", DateDefault.Min, true),
        InlineData("2024-02-03", "15:07:12 UTC", DateDefault.Max, true),
        InlineData("2/4/2025", "6:35 PM utc", DateDefault.Min, true),
        InlineData("2/4/2025", "6:35 PM utc", DateDefault.Max, true),
    ]
    public void ConvertDateStrToDateTime(string? date, string? time, DateDefault def, bool success)
    {
        DateTime result = ConvertPrimitive.ConvertDate(date, time, def);

        if (success)
        {
            DateTime expected = time is null ? DateTime.Parse(date!) : DateTime.Parse(date! + " " + time.ToLower().Split("utc")[0]!);
            Assert.Equal(expected, result);
        }
        else
        {
            Assert.Equal(result, def.DateTimeDefault());
        }
    }
    #endregion

    #region ConvertDate(string? date, DateTimeDefault) 
    [
        Theory,
        InlineData(null, DateDefault.Min),
        InlineData(null, DateDefault.Max),
        InlineData("February 3, 2025 15:07:12", DateDefault.Min),
        InlineData("February 3, 2025 15:07:12", DateDefault.Max),
        InlineData("2024-02-03, 2024 12:03:59", DateDefault.Min),
        InlineData("2024-02-03, 2024 12:03:59", DateDefault.Max),
    ]
    public void ConvertNullableStringToDateTime(string? input, DateDefault def)
    {
        DateTime actual = ConvertPrimitive.ConvertDate(input, def);
        DateTime expected = DateTime.TryParse(input, out DateTime d) ? d : def.DateTimeDefault();
        Assert.Equal(expected, actual);
    }
    #endregion
}