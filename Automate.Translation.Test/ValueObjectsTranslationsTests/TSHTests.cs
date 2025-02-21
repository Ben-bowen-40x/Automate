using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.Test;

public class TSHTests
{
    #region ContentsJoined(string)
    [
        Theory,
        InlineData("stuff, things, stuff and things \r\n\r\n\n\r\r\n ,,, things and stuff", "stuff| things| stuff and things| things and stuff"),
        InlineData("stuff, things, stuff and things \r\n\r\n\r\r\n ,,, things and stuff", "stuff| things| stuff and things| things and stuff"),
    ]
    public void TestContentsJoiner(string input, string expected)
    {
        var result = TSH.ContentsJoined(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region ReplaceCsvAwkward(string input, string? joiner)
    [
        Theory,
        InlineData("stuff, things, stuff and things \r\n\r\n\r\r\n ,,, things and stuff", "|", "stuff| things| stuff and things ||||||| ||| things and stuff"),
        InlineData("stuff, things, stuff and things \r\n\r\n\r\r\n ,,, things and stuff", null, "stuff things stuff and things   things and stuff"),

    ]
    public void ReplaceCsvAwkwardTest(string input, string? joiner, string expected)
    {
        var result = TSH.ReplaceCsvAwkward(input, joiner);
        Assert.Equal(expected, result);
    }
    #endregion
}
