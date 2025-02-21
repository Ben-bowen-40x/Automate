using Automate.Application.InfrastructureValueObjects;
using Automate.Domain.ValueObjects;
using Automate.Translation.LeafTranslate;
using Automate.Translation.PhoneNumTranslate;
using NSubstitute;

namespace Automate.Translation.Test;

public class LeafThreadTranslateTest
{
    #region GetFirstMessage
    [
        Theory,
        InlineData(new int[] { 2024, 03, 25, 15, 38, 02 }),
        InlineData(new int[] { 2024, 09, 21, 11, 12, 20 }),
        InlineData(new int[] { 2025, 01, 21, 11, 12, 20 }),
        InlineData(new int[] { 2023, 09, 23, 23, 02, 15 }),
        InlineData(new int[] { 2022, 09, 21, 11, 12, 20 }),
    ]
    public void GetFirstMessageTest(int[] dateInts)
    {
        // Assemble primitives
        const string ingress = "ingress"; // Direction has to be ingress for the date to be accepted
        DateTime thedate = new(dateInts[0], dateInts[1], dateInts[2], dateInts[3], dateInts[4], dateInts[5]);

        // Assemble types -- Msg arr
        Msg first = Substitute.For<Msg>();
        first.Creation = (thedate);
        first.Direction = (ingress);
        Msg second = Substitute.For<Msg>();
        second.Creation = (thedate + TimeSpan.FromHours(48));
        second.Direction = (ingress);
        Msg third = Substitute.For<Msg>();
        third.Creation = (thedate + TimeSpan.FromDays(4) + TimeSpan.FromHours(3) + TimeSpan.FromMinutes(4));
        second.Direction = (ingress);
        List<Msg> mockArr = [first, second, third];

        // Act
        var actual = LeafThreadTranslate.GetFirstMessage(mockArr);

        // Assertions
        Assert.Equal(first.Creation, actual.Creation);
    }
    #endregion

    #region VerifyMessages
    [Fact]
    public void VerifyMessagesTest()
    {
        // Assemble primitives
        Msg mgs = Substitute.For<Msg>();

        // Act 
        var actual = LeafThreadTranslate.VerifyMessages([mgs]);

        // Assert
        Assert.NotNull(actual);
        Assert.NotEqual(LeafThreadTranslate.DefaultMsgArr(), actual);
    }
    #endregion

    #region GetPhoneNumbers
    [
        Theory,
        InlineData(null),
        InlineData("9876543210"),
        InlineData("4567891230"),
        InlineData("7891234560"),
    ]
    public void ExtractPhoneNumberTest(string? phone)
    {
        // Assemble 
        Prospect mock = Substitute.For<Prospect>();
        mock.Cellphone = phone;
        PhoneNumber expected = PhoneNumberTranslate.Translate(phone);

        // Act
        var actual = LeafThreadTranslate.ExtractPhoneNumber(mock);

        // Assert
        Assert.Equal(expected.Number, actual.Number);
    }
    #endregion

    
}
