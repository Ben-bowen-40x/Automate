using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;
using NSubstitute;

namespace Automate.Domain.Test.Discrepancy;

public class MatchDiscrepancyCallsTests
{
    #region BestMatch
    [
        Theory,
        InlineData(2089071470),
        InlineData(9458761346),
        InlineData(7945823164),
    ]
    public void BestMatchTest(long numb)
    {
        // Assemble
        DateTime dt = DateTime.Now - TimeSpan.FromDays(180);
        TimeSpan dur = TimeSpan.FromSeconds(15.15);

        // Create items that are going to be compared
        IDiscrepancyCall matchesOne = Substitute.For<IDiscrepancyCall>();
        matchesOne.Number.Returns(new PhoneNumber(numb));
        matchesOne.Date.Returns(dt);
        matchesOne.Duration.Returns(dur);

        var oneDtFactor = dt + TimeSpan.FromSeconds(59);
        var oneDurFactor = dur + TimeSpan.FromSeconds(1);
        IDiscrepancyCall one = Substitute.For<IDiscrepancyCall>();
        one.Number.Returns(new PhoneNumber(numb));
        one.Date.Returns(oneDtFactor);
        one.Duration.Returns(oneDurFactor);

        var twoDtFactor = dt + TimeSpan.FromSeconds(61);
        var twoDurFactor = dur + TimeSpan.FromSeconds(2);
        IDiscrepancyCall two = Substitute.For<IDiscrepancyCall>();
        two.Number.Returns(new PhoneNumber(numb));
        two.Date.Returns(twoDtFactor);
        two.Duration.Returns(twoDurFactor);

        IDiscrepancyCall three = Substitute.For<IDiscrepancyCall>();
        three.Number.Returns(new PhoneNumber(numb));
        three.Date.Returns(dt + TimeSpan.FromSeconds(62));
        three.Duration.Returns(dur + TimeSpan.FromSeconds(3));

        IDiscrepancyCall four = Substitute.For<IDiscrepancyCall>();
        four.Number.Returns(new PhoneNumber(numb));
        four.Date.Returns(dt + TimeSpan.FromSeconds(63));
        four.Duration.Returns(dur + TimeSpan.FromSeconds(4));

        IDiscrepancyCall five = Substitute.For<IDiscrepancyCall>();
        five.Number.Returns(new PhoneNumber(numb));
        five.Date.Returns(dt + TimeSpan.FromSeconds(64));
        five.Duration.Returns(dur + TimeSpan.FromSeconds(5));

        List<IDiscrepancyCall> callList1 = [one, two, three, four, five];
        List<IDiscrepancyCall> callList2 = [two, three, four, five];

        IDiscrepancyCall expected1 = Substitute.For<IDiscrepancyCall>();
        expected1.Number.Returns(new PhoneNumber(numb));
        expected1.Date.Returns(oneDtFactor);
        expected1.Duration.Returns(oneDurFactor);
        IDiscrepancyCall expected2 = Substitute.For<IDiscrepancyCall>();
        expected2.Number.Returns(new PhoneNumber(numb));
        expected2.Date.Returns(twoDtFactor);
        expected2.Duration.Returns(twoDurFactor);

        // Act
        IDiscrepancyCall actual1 = MatchDiscrepancyCalls.BestMatch(matchesOne, callList1);
        IDiscrepancyCall actual2 = MatchDiscrepancyCalls.BestMatch(matchesOne, callList2);

        // Assert
        Assert.Equal(actual1.Number.Number, expected1.Number.Number);
        Assert.Equal(actual1.Date, expected1.Date);
        Assert.Equal(actual1.Duration, expected1.Duration);

        Assert.Equal(actual2.Number.Number, expected2.Number.Number);
        Assert.Equal(actual2.Date, expected2.Date);
        Assert.Equal(actual2.Duration, expected2.Duration);
    }
    #endregion

    #region MinuteMatches
    [
        Theory,
        InlineData(new int[] { 2024, 06, 15, 10, 59, 59 }),
        InlineData(new int[] { 2024, 06, 15, 10, 00, 00 }),
    ]
    public void MinuteMatchesTest(int[] dateints)
    {
        // Assemble
        DateTime date = new(dateints[0], dateints[1], dateints[2], dateints[3], dateints[4], dateints[5]);
        DateTime matchDate1 = date - TimeSpan.FromMinutes(1);
        DateTime matchDate2 = date + TimeSpan.FromMinutes(1);
        DateTime noMatchDate1 = date - TimeSpan.FromMinutes(2);
        DateTime noMatchDate2 = date + TimeSpan.FromMinutes(2);

        IDiscrepancyCall lead = Substitute.For<IDiscrepancyCall>();
        lead.Date.Returns(date);
        IDiscrepancyCall match1 = Substitute.For<IDiscrepancyCall>();
        match1.Date.Returns(matchDate1);
        IDiscrepancyCall match2 = Substitute.For<IDiscrepancyCall>();
        match2.Date.Returns(matchDate2);
        IDiscrepancyCall noMatch1 = Substitute.For<IDiscrepancyCall>();
        noMatch1.Date.Returns(noMatchDate1);
        IDiscrepancyCall noMatch2 = Substitute.For<IDiscrepancyCall>();
        noMatch2.Date.Returns(noMatchDate2);

        // Act
        bool leadMatch1Actual = MatchDiscrepancyCalls.MinuteMatches(lead, match1);
        bool leadMatch2Actual = MatchDiscrepancyCalls.MinuteMatches(lead, match2);
        bool leadNoMatch1Actual = MatchDiscrepancyCalls.MinuteMatches(lead, noMatch1);
        bool leadNoMatch2Actual = MatchDiscrepancyCalls.MinuteMatches(lead, noMatch2);

        // Assert
        Assert.True(leadMatch1Actual);
        Assert.True(leadMatch2Actual);
        Assert.False(leadNoMatch1Actual);
        Assert.False(leadNoMatch2Actual);
    }
    #endregion

    #region ClosestDuration
    [
        Theory,
        InlineData(20),
        InlineData(94),
        InlineData(79),
    ]
    public void ClosestDurationTest(int num)
    {
        // Assemble
        TimeSpan leadDur = TimeSpan.FromSeconds(num);
        TimeSpan matchDur = TimeSpan.FromSeconds(num + 12);
        TimeSpan compDur = TimeSpan.FromSeconds(num + 24);
        PhoneNumber number = new(9876541230);

        var lead = Substitute.For<IDiscrepancyCall>();
        lead.Duration.Returns(leadDur);
        lead.Number.Returns(number);
        lead.Billable.Returns(true);
        var match = Substitute.For<IDiscrepancyCall>();
        match.Duration.Returns(matchDur);
        match.Number.Returns(number);
        match.Billable.Returns(true);
        var compare = Substitute.For<IDiscrepancyCall>();
        compare.Duration.Returns(compDur);
        compare.Number.Returns(number);
        compare.Billable.Returns(false);

        // Act
        IDiscrepancyCall actual = MatchDiscrepancyCalls.ClosestDuration(lead, match, compare);

        // Assert
        Assert.Equal(match.Number.Number, actual.Number.Number);
        Assert.Equal(match.Duration, actual.Duration);
    }
    #endregion

    #region PhoneDateDurMatch
    [
        Theory,
        InlineData(true, true),
        InlineData(true, false),
        InlineData(false, true),
        InlineData(false, false),
    ]
    public void PhoneDateDurMatch(bool matchBill, bool compBill)
    {
        // Assemble
        var match = Substitute.For<IDiscrepancyCall>();
        match.Billable.Returns(matchBill);
        match.Date.Returns(DateTime.MaxValue);
        match.Number.Returns(new PhoneNumber(0));
        match.Duration.Returns(TimeSpan.FromSeconds(0));
        var comp = Substitute.For<IDiscrepancyCall>();
        comp.Billable.Returns(compBill);
        comp.Date.Returns(DateTime.MaxValue);
        comp.Number.Returns(new PhoneNumber(0));
        comp.Duration.Returns(TimeSpan.FromSeconds(0));

        // Act
        var actual = MatchDiscrepancyCalls.PhoneDateDurMatch(match, match, comp);

        // Assert
        if (matchBill || (!matchBill && !compBill))
            Assert.Equal(actual, match);
        else if (compBill)
            Assert.Equal(actual, comp);
    }
    #endregion
}
