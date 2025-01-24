using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.Test;

public class TranslationLayerTests
{
    [
        Theory,
        InlineData(null, null, false),
        InlineData(null, "6:35 PM utc", false),
        InlineData("February 3, 2025", null, true),
        InlineData("2024-02-03", "15:07:12 UTC", true),
        InlineData("2/4/2025", "6:35 PM utc", true),
    ]
    public void ConvertDateStrToDateTime_ProperlyConverts(string? date, string? time, bool success)
    {
        DateTime result = MessageInterfaceTranslate.ConvertDate(date, time);

        if (success)
        {
            DateTime expected = time is null ? DateTime.Parse(date!) : DateTime.Parse(date! + " " + time.ToLower().Split("utc")[0]!);
            Assert.Equal(expected, result);
        }
        else
        {
            Assert.Equal(result, DateTime.MinValue);
        }
    }

    [
        Theory,
        InlineData("stuff, things, stuff and things \r\n\r\n\n\r\r\n ,,, things and stuff", "stuff| things| stuff and things| things and stuff")
    ]
    public void TestContentsJoiner(string input, string expected)
    {
        var result = TSH.ContentsJoined(input);
        Assert.Equal(expected, result);
    }
}