using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Translation.Test;

public class MessageInterfaceTranslateTest
{
    #region VerifyNumber(string? source, string? stringNum)
    [
        Theory,
        InlineData(null, null, 0),
        InlineData(null, "8015558142", 0),
        InlineData("null", "8015558142", 8015558142),
    ]
    public void VerifyNumberTest(string? source, string? stringNum, long expected)
    {
        // Assemble
        PhoneNumber expectedPh = PhoneNumberTranslate.Translate(expected);

        // Act
        PhoneNumber actual = MessageInterfaceTranslate.VerifyNumber(source, stringNum);

        // Assert
        Assert.Equal(expectedPh.Number, actual.Number);
    }
    #endregion

    #region VerifyContents(string? contents)
    [
        Theory,
        InlineData(null, ""),
        InlineData("null", "null"),
        InlineData("This, is, \n,\ncontent", "This| is| content"),
    ]
    public void VerifyContentsTest(string? contents, string expected)
    {
        // Assemble & Act
        var actual = MessageInterfaceTranslate.VerifyContents(contents);

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion

    #region VerifySource(string? source)
    [
        Theory,
        InlineData(null, ""),
        InlineData("z:", ""),
        InlineData("z:Thisisthesource", "Thisisthesource"),
        InlineData("z:This,is,the\nsource", "Thisisthesource"),
    ]
    public void VerifySourceTest_NoComponent(string? source, string expected)
    {
        // Assemble & Act
        var actual = MessageInterfaceTranslate.VerifySource(source);

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion
    
    #region VerifySource(string? source, SourceComponent component)
    [
        Theory,
        InlineData(null, SourceComponent.Gclid, ""),
        InlineData(null, SourceComponent.Msclid, ""),
        InlineData("https://thisisawebsite.com/thisisaurlthingy?msclid=thisisthemsclickid/stuff/things/stuffandthings", SourceComponent.Msclid, "thisisthemsclickid"),
        InlineData("https://thisisawebsite.com/thisisaurlthingy?msclid=thisisthemsclickid", SourceComponent.Msclid, "thisisthemsclickid"),
        InlineData("https://thisisawebsite.com/thisisaurlthingy?gclid=thisisthegclickid/stuff/things/stuffandthings", SourceComponent.Gclid, "thisisthegclickid"),
        InlineData("https://thisisawebsite.com/thisisaurlthingy?gclid=thisisthegclickid", SourceComponent.Gclid, "thisisthegclickid"),
    ]
    public void VerifySourceTest(string? source, SourceComponent component, string expected)
    {
        // Assemble & Act
        var actual = MessageInterfaceTranslate.VerifySource(source, component);

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion
}
