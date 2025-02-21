using Automate.Domain.ValueObjects;

namespace Automate.Domain.Test;

public class PhoneNumberTests
{
    private const string _skipMsg = "This test is being skipped for now";

    #region Inline Data
    [
        Theory
        //(Skip = _skipMsg)
        ,
        InlineData(4319289474, 4319289474, true),
        InlineData(4319289474, 3302192745, false),
        InlineData(9041744225, 3302192745, false),
        InlineData(5598418890, 5598418890, true),
    ]
    #endregion
    public void PhoneNumber_IEqualityComparerProperlyCompares(long phone1, long phone2, bool expected)
    {
        PhoneNumber num1 = new(phone1);
        PhoneNumber num2 = new(phone2);
        bool numEquality = num1.Number.Equals(num2.Number);
        Assert.Equal(numEquality, expected);
    }

    #region Inline Data
    [
        Theory
        //(Skip = _skipMsg)
        ,
        InlineData("10000000000", 0),
        InlineData("10000129911", 0),
        InlineData("10807110292", 0),
        InlineData("11000000000", 0),
        InlineData("11045003537", 0),
        InlineData("11432681874", 0),
        InlineData("11507394366", 0),
        InlineData("11631511550", 0),
        InlineData("11791966986", 0),
        InlineData("12012000307", 2012000307),
        InlineData("(867) 638-4943", 8676384943),
        InlineData("(223) 382-3138", 2233823138),
        InlineData("(256) 116-1572", 2561161572),
        InlineData("+1(867) 638-4943", 8676384943),
        InlineData("+1(223) 382-3138", 2233823138),
        InlineData("+1(256) 116-1572", 2561161572),
        InlineData("+1(639) 161-7221", 6391617221)
    ]
    #endregion
    public void PhoneNumber_CtorWorksOnString(string input, long expected)
    {
        // Arrange       
        // Act           
        PhoneNumber number = new(input);
        PhoneNumber number2 = new(number);

        // Assert
        Assert.Equal(expected, number.Number);
        Assert.Equal(expected, number2.Number);
        Assert.Equal(number.Number, number2.Number);
    }

    #region Inline Data
    [
        Theory
        //(Skip = _skipMsg)
        ,
        InlineData(10000000000, 0),
        InlineData(10000129911, 0),
        InlineData(10807110292, 0),
        InlineData(11000000000, 0),
        InlineData(11045003537, 0),
        InlineData(11432681874, 0),
        InlineData(11507394366, 0),
        InlineData(12012070500, 2012070500),
        InlineData(12012070755, 2012070755),
        InlineData(12012070836, 2012070836),
        InlineData(12012073343, 2012073343),
   ]
    #endregion
    public void PhoneNumber_CtorWorksOnLong(long input, long expected)
    {
        // Arrange       
        // Act           
        PhoneNumber number = new(input);
        PhoneNumber number2 = new(number);

        // Assert
        Assert.Equal(expected, number.Number);
        Assert.Equal(expected, number2.Number);
        Assert.Equal(number.Number, number2.Number);
    }

    #region Inline Data
    [
        Theory
        //(Skip = _skipMsg)
        ,
        InlineData("(867) 638-4943", 8676384943),
        InlineData("(223) 382-3138", 2233823138),
        InlineData("(256) 116-1572", 2561161572),
        InlineData("867) 638-4943", 8676384943),
        InlineData("223) 382-3138", 2233823138),
        InlineData("256) 116-1572", 2561161572),
        InlineData("+1(929) 853-8563", 9298538563),
        InlineData("+1(651) 160-0526", 6511600526),
        InlineData("+1(260) 138-2691", 2601382691),
        InlineData("+1(639) 161-7221", 6391617221),
        InlineData("10000000000", 0),
        InlineData("10000129911", 0),
        InlineData("10807110292", 0),
        InlineData("11000000000", 0),
        InlineData("11045003537", 0),
        InlineData("11432681874", 0),
        InlineData("1100000000", 0),
        InlineData("1201207686", 0),
        InlineData("1201207735", 0),
        InlineData("1201207924", 0),
        InlineData("(908) 355-333", 0),
        InlineData("(330) 219-274", 0),
        InlineData("(639) 161-722", 0),
        InlineData("+1(867) 638-494", 0),
        InlineData("+1(908) 355-333", 0),
        InlineData("+1(330) 219-274", 0),
        InlineData("+1(639) 161-722", 0),
        InlineData(null, 0),
        InlineData("No Number", 0),
        InlineData("Jun 3, 2024 at 9:19 PM EDT", 0)
    ]
    #endregion
    public void PhoneNumber_TryParseWorksOnString(string? input, long expected)
    {
        bool worked = PhoneNumber.TryParse(input, out PhoneNumber actual);
        if (expected != 0)
            Assert.True(worked);
        Assert.Equal(expected, actual.Number);
    }
}
