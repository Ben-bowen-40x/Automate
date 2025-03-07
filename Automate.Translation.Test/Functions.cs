namespace Automate.Translation.Test;

public class Functions
{
    public static DateTimeOffset GetExpectedDateTimeOffset(int dateInt, DateTime newDate)
    {
        return new(newDate - TimeSpan.FromHours(dateInt), TimeSpan.FromHours(0));
    }

    public static DateTime IntsToDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        DateTime value = year == 0 || month == 0 || day == 0
            ? DateTime.MinValue
            : new DateTime(year, month, day, hour, minute, second);

        return value;
    }
    public static DateTimeOffset IntsToDto(int year, int month, int day, int hour, int minutes, int seconds, int offset)
    {
        DateTimeOffset value = new(IntsToDateTime(year, month, day, hour, minutes, seconds), TimeSpan.FromHours(offset));

        return value;
    }
}
