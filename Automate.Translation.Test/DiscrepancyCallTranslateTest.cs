using Automate.Translation.DiscrepancyTranslate;

namespace Automate.Translation.Test;

public class DiscrepancyCallTranslateTest
{
    #region VerifyNotes
    [
        Theory,
        InlineData("this, is, the note\n\n\n\r\r\n\r\n\rwith random stuff", "this| is| the note| with random stuff"),
        InlineData(null, ""),
    ]
    public void VerifyNotesTest(string? note, string expected)
    {
        // Assemble & Act
        var actual = DiscrepancyCallTranslate.VerifyNotes(note);

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion

    #region GetDuration(string?)
    [
        Theory,
        InlineData("00:14:45"),
        InlineData("175"),
        InlineData(null),
        InlineData(""),
    ]
    public void GetDurationString(string? duration)
    {
        // Assemble
        TimeSpan expected = TimeSpan.TryParse(duration, out expected) & !int.TryParse(duration, out int durInt)
            ? expected
            : TimeSpan.FromSeconds(durInt);

        // Act
        TimeSpan actual = DiscrepancyCallTranslate.GetDuration(duration);

        // Assert
        Assert.Equal(actual, expected);
    }
    #endregion

    #region GetDuration(int?)
    [
        Theory,
        InlineData(14),
        InlineData(175),
        InlineData(null),
    ]
    public void GetDurationInt(int? duration)
    {
        // Assemble
        TimeSpan expected = !int.TryParse(duration.ToString(), out int durInt)
            ? new(0)
            : TimeSpan.FromSeconds(durInt);

        // Act
        TimeSpan actual = DiscrepancyCallTranslate.GetDuration(duration);

        // Assert
        Assert.Equal(actual, expected);
    }
    #endregion
}
