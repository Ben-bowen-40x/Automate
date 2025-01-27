using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.Test;

public class TSHTests
{
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
