using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;
using NSubstitute;

namespace Automate.Domain.Test;

public class DiscrepancyAnalysisTests
{
    #region BestMatch
    [
        Theory,
        InlineData(),
    ]
    public void BestMatchTest()
    {
        // Assemble
        // Act
        // Assert
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
        InlineData(),
    ]
    public void ClosestDurationTest()
    {
        // Assemble
        // Act
        // Assert
    }
    #endregion

    #region PhoneDateDurMatch
    [
        Theory,
        InlineData(),
    ]
    public void PhoneDateDurMatch()
    {
        // Assemble
        // Act
        // Assert
    }
    #endregion
}
