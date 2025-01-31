using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.Test.ValueObjectsTranslationsTests;
using Automate.Translation.ValueObjectsTranslations;
using CSharpFunctionalExtensions;
using NSubstitute;

namespace Automate.Translation.Test;

public class MessageInterfaceTranslateTest
{
    #region IMsgDTONumberLongTranslationTest
    [
        Theory,
        InlineData(
        // Phone Number
        9876543210,
        // Date:: year, month, day, hour, minute, second
        new int[] { 2024, 7, 12, 10, 45, 0 },
        // Contents
        "These are contents, with all kinds of \n weird stuff in it \n\"In fact, you might wonder why there are weird stuff in here.\" That is all.",
        // Source
        "z:This is the source")
    ]
    public void IMsgDTONumberLongTranslationTest(long number, int[] dateInts, string? contents, string? source)
    {
        // Set up primitives
        DateTimeOffset date = new(ConvertDateTimeOffsetTests.MakeDateFromIntArray(dateInts[0], dateInts[1], dateInts[2], dateInts[3], dateInts[4], dateInts[5]), TimeSpan.FromHours(0));
        var contentsGood = MessageInterfaceTranslate.VerifyContents(contents);
        var sourceGood = MessageInterfaceTranslate.VerifySource(source);
        var phNumber = PhoneNumberTranslate.Translate(number);

        // Set up
        IMsgDTONumberLong mock = Substitute.For<IMsgDTONumberLong>();
        mock.Number.Returns(number);
        mock.Date.Returns(date);
        mock.Contents.Returns(contentsGood);
        mock.Source.Returns(sourceGood);

        // Set up expected value
        IMessage expected = Substitute.For<IMessage>();
        expected.Number.Returns(phNumber);
        expected.Date.Returns(date);
        expected.Contents.Returns(contentsGood);
        expected.Source.Returns(sourceGood);

        // Act
        var actual = mock.Translate();

        // Assert
        Assert.Equal(expected.Number.Number, actual.Number.Number);
        Assert.Equal(expected.Date, actual.Date);
        Assert.Equal(expected.Contents, actual.Contents);
        Assert.Equal(expected.Source, actual.Source);
    }
    #endregion

    #region IMsgStrDateTimeOffsetTranslationTest
    [
        Theory,
        InlineData(
        // Phone Number
        "9876543210",
        // Date:: year, month, day, hour, minute, second
        new int[] { 2024, 7, 12, 10, 45, 0 },
        // Contents
        "These are contents, with all kinds of\nweird stuff in it\n\"In fact, you might wonder why there are weird stuff in here.\" That is all.",
        // Source
        "z:This is the source")
    ]
    public void IMsgStrDateTimeOffsetTranslationTest(string number, int[] dateInts, string? contents, string? source)
    {
        // Set up primitive conversion
        PhoneNumber numberConverted = PhoneNumberTranslate.Translate(number);
        DateTimeOffset date = new(ConvertDateTimeOffsetTests.MakeDateFromIntArray(dateInts[0], dateInts[1], dateInts[2], dateInts[3], dateInts[4], dateInts[5]));
        string contentsGood = MessageInterfaceTranslate.VerifyContents(contents);
        string sourceGood = MessageInterfaceTranslate.VerifySource(source);
        PhoneNumber phNumber = PhoneNumberTranslate.Translate(number);

        // Set up
        IMsgStrDateTimeOffset mock = Substitute.For<IMsgStrDateTimeOffset>();
        mock.Number.Returns(number);
        mock.Date.Returns(date);
        mock.Contents.Returns(contentsGood);
        mock.Source.Returns(sourceGood);

        // Set up expected
        IMessage expected = Substitute.For<IMessage>();
        expected.Number.Returns(numberConverted);
        expected.Date.Returns(date);
        expected.Contents.Returns(contentsGood);
        expected.Source.Returns(sourceGood);

        // Act
        IMessage actual = mock.Translate();

        // Assert
        Assert.Equal(expected.Number.Number, actual.Number.Number);
        Assert.Equal(expected.Date, actual.Date);
        Assert.Equal(expected.Contents, actual.Contents);
        Assert.Equal(expected.Source, actual.Source);
    }
    #endregion

    #region IMsgNoTimeStrUtcTranslationTest
    [
        Theory,
        InlineData(
        // Phone Number
        "9876543210",
        // Date:: year, month, day, hour, minute, second
        "2024-7-12 10:45:00",
        // Contents
        "These are contents, with all kinds of\nweird stuff in it\n\"In fact, you might wonder why there are weird stuff in here.\" That is all.",
        // Source
        "z:This is the source")
    ]
    public void IMsgNoTimeStrUtcTranslationTest(string numberStr, string dateTimeStr, string contents, string source)
    {
        // Set up Primitives
        PhoneNumber number = PhoneNumberTranslate.Translate(numberStr);
        DateTime interDate = ConvertPrimitive.ConvertDate(dateTimeStr, null, DateTimeDefault.Min);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(interDate, TimeSpan.FromTicks(0)); // This type is already in UTC
        string contentsGood = MessageInterfaceTranslate.VerifyContents(contents);
        string sourceGood = MessageInterfaceTranslate.VerifySource(source);

        // Set up 
        IMsgNoTimeStrUtc mock = Substitute.For<IMsgNoTimeStrUtc>();
        mock.NumberStr.Returns(numberStr);
        mock.DateTimeStr.Returns(dateTimeStr);
        mock.Contents.Returns(contentsGood);
        mock.Source.Returns(sourceGood);

        // Set up expected
        IMessage expected = Substitute.For<IMessage>();
        expected.Number.Returns(number);
        expected.Date.Returns(date);
        expected.Contents.Returns(contentsGood);
        expected.Source = sourceGood;

        // Act
        var actual = mock.Translate();

        // Assert
        Assert.Equal(expected.Number.Number, actual.Number.Number);
        Assert.Equal(expected.Date, actual.Date);
        Assert.Equal(expected.Contents, actual.Contents);
        Assert.Equal(expected.Source, actual.Source);
    }
    #endregion

    #region IMsgDTOStrTranslationTest
    [
        Theory,
        InlineData(
        // Phone Number
        "9876543210",
        // Date:: year, month, day, hour, minute, second
        "2024-7-12 10:45:00",
        // Contents
        "These are contents, with all kinds of\nweird stuff in it\n\"In fact, you might wonder why there are weird stuff in here.\" That is all.",
        // Source
        "z:This is the source",
        DateTimeDefault.Min)
    ]
    public void IMsgDTOStrTranslationTest(string numberStr, string dateTimeOffsetStr, string contents, string source, DateTimeDefault dtDefault)
    {
        // Set Up primitives
        PhoneNumber number = PhoneNumberTranslate.Translate(numberStr);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(dateTimeOffsetStr, dtDefault);
        string contentsGood = MessageInterfaceTranslate.VerifyContents(contents);
        string sourceGood = MessageInterfaceTranslate.VerifySource(source);

        // Set up 
        IMsgDTOStr mock = Substitute.For<IMsgDTOStr>();
        mock.Number.Returns(numberStr);
        mock.DateTimeOffsetStr.Returns(dateTimeOffsetStr);
        mock.Contents.Returns(contentsGood);
        mock.Source = sourceGood;

        // Actual
        IMessage actual = Substitute.For<IMessage>();
        actual.Number.Returns(number);
    }
    #endregion

    #region IMsgDTOStrIsolateSourceTranslationTest
    [
        Theory,
        InlineData()
    ]
    public void IMsgDTOStrIsolateSourceTranslationTest() { }
    #endregion

    #region IMsgDTOStrNonEmptySourceTranslationTest
    [
        Theory,
        InlineData()
    ]
    public void IMsgDTOStrNonEmptySourceTranslationTest() { }
    #endregion

    #region IMsgZoneEnumStrTranslationTest
    [
        Theory,
        InlineData()
    ]
    public void IMsgZoneEnumStrTranslationTest() { }
    #endregion

}
