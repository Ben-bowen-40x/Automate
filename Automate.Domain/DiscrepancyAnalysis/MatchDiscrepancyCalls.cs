using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;

namespace Automate.Domain.DiscrepancyAnalysis;

public class MatchDiscrepancyCalls
{
    internal static IDiscrepancyCall _defaultCall = new DiscrepancyCall(new(0), default, DateTime.MaxValue, TimeSpan.FromMicroseconds(0), string.Empty);

    #region Public
    public static List<IMatchingLeads> MatchLeads(List<IDiscrepancyCall> billedLeads, List<IDiscrepancyCall> comparisonLeads)
    {
        // Prepare the log for this execution
        string location = GetFullName.GetMemberName(new MatchDiscrepancyCalls(), nameof(MatchLeads));
        StringLogger.AddLog($"Started log for {location}");

        // Prepare result
        List<IMatchingLeads> result = new(billedLeads.Count);

        // Iterate through billedLead to expose its values individually
        foreach (var billed in billedLeads)
        {
            // Create a list of comparison leads whose phone number matches the current lead
            List<IDiscrepancyCall> phoneMatch = comparisonLeads
                .Where(l => l.Number.Number == billed.Number.Number)
                .ToList();

            // Find the lead that is chronologically soonest after the current lead. The comparison day necessarily must be equal to the billable lead
            List<IDiscrepancyCall> dayMatches = phoneMatch
                .Where(p => MatchDates(p, billed))
                .ToList();
            List<IDiscrepancyCall> input = dayMatches.Count == 0
                ? phoneMatch
                : dayMatches;

            IDiscrepancyCall match = BestMatch(billed, input);

            // Find out if there are billable calls with matching phone number before the lead
            List<IDiscrepancyCall> billableBefore = phoneMatch
                .Where(m => m.Billable && DateTime.Compare(m.Date, billed.Date) < 0)
                .ToList();

            // Add the result
            result.Add(new MatchingLeads(billed, match, billableBefore.Count > 0));

            LogIrregularities(location, billed, dayMatches, input);
        }

        // Conclude log for this execution
        StringLogger.AddLog($"Ended log for {location}",
            $"Total calls being compared: {billedLeads.Count}");

        // Return result
        return result;

        // Local Functions
        static bool MatchDates(IDiscrepancyCall p, IDiscrepancyCall lead) =>
            p.Date.Year == lead.Date.Year && p.Date.Date.DayOfYear == lead.Date.Date.DayOfYear;
        static void LogIrregularities(string location, IDiscrepancyCall billed, List<IDiscrepancyCall> dayMatches, List<IDiscrepancyCall> input)
        {
            // Log irregularities
            if (dayMatches.Count == 0)
                StringLogger.AddLog(
                    "Strange interaction in:",
                    location,
                    "Could not find any records matching the date of the current call. No calls could be found that match the year and day-of-the-year with the following billed call:",
                    billed.ToString());
            else if (input.Count == 0)
                StringLogger.AddLog(
                    "Strange interaction in:",
                    location,
                    "Could not find any records matching the current call by phone number. This is the call:",
                    billed.ToString());
        }
    }

    #endregion

    #region Internal
    internal static IDiscrepancyCall BestMatch(IDiscrepancyCall billed, IList<IDiscrepancyCall> phMatchesSameDay)
    {
        if (phMatchesSameDay.Count == 0)
            return new DiscrepancyCall(new(0), false, DateTime.MinValue, TimeSpan.Zero, "");

        // Iterate through each lead to find the one whose minute and day of the year match
        List<IDiscrepancyCall> result = phMatchesSameDay
            .Where(match => MinuteMatches(billed, match))
            .ToList();

        // Check results for matches
        if (result.Count != 1)
        {
            bool resultIsEmpty = result.Count == 0;
            IDiscrepancyCall closest = resultIsEmpty ? phMatchesSameDay[0] : result[0];
            IList<IDiscrepancyCall> iteration = resultIsEmpty ? phMatchesSameDay : result;
            foreach (var r in iteration)
            {
                closest = ClosestDuration(billed, r, closest);
            }

            // Log which call was chosen, but only if there was a legitimate question as to whether the right one would be found and only if the correct record may not have been found
            bool foundCorrectCall =
                billed.Number.Number != _defaultCall.Number.Number
                && closest.Number.Number == billed.Number.Number
                && closest.Duration <= billed.Duration + TimeSpan.FromSeconds(5)
                && closest.Duration >= billed.Duration - TimeSpan.FromSeconds(5);
            StringLogger.EndAlludeLog(!foundCorrectCall, "Call that ended up being chosen, Phone number, Date, Duration:", $"{closest.Number.Number}, {closest.Date}, {closest.Duration}");

            result = [closest];
        }

        // Return result
        return result[0];
    }

    internal static bool MinuteMatches(IDiscrepancyCall lead, IDiscrepancyCall match) =>
        match.Date + TimeSpan.FromMinutes(1) >= lead.Date && match.Date - TimeSpan.FromMinutes(1) <= lead.Date;
        //match.Date.Minute + 1 == lead.Date.Minute
        //|| match.Date.Minute - 1 == lead.Date.Minute
        //|| match.Date.Minute == lead.Date.Minute;

    internal static IDiscrepancyCall ClosestDuration(IDiscrepancyCall lead, IDiscrepancyCall match, IDiscrepancyCall compare) => lead.Duration switch
    {
        // These are the most likely
        // Comparison is shorter , Match is longer
        var l when compare.Duration < l && l < match.Duration => match,
        // Match is shorter , Comparison is longer
        var l when match.Duration < l && l < compare.Duration => compare,
        // Lead is first, then Match, then Comparison
        var l when l < match.Duration && l < compare.Duration && match.Duration < compare.Duration => match,
        // Lead is first, then Comparison, then Match
        var l when l < compare.Duration && l < match.Duration && compare.Duration < match.Duration => compare,

        // These are a bit less likely
        // Comparison is shorter and Match equals Lead
        var l when compare.Duration < l && l == match.Duration => match,
        // Match is shorter and Comparison equals Lead
        var l when match.Duration < l && l == compare.Duration => compare,
        // Lead and Match is equal and Comparison is longer
        var l when l == match.Duration && l < compare.Duration => match,
        // Lead and Comparison is equal and Match is longer
        var l when l == compare.Duration && l < match.Duration => compare,

        // This is where we get into must less common territory
        // Comparison is first, then Match, then Lead
        var l when compare.Duration < match.Duration && compare.Duration < l && match.Duration < l => match,
        // Match is first, then Comparison, then lead
        var l when match.Duration < compare.Duration && match.Duration < l && compare.Duration < l => compare,
        // Lead is first, then Comparison and Match are equal
        var l when compare.Duration == match.Duration && l < compare.Duration => PhoneDateDurMatch(lead, match, compare),
        // Comparison and Match are equal and Lead is longest
        var l when compare.Duration == match.Duration && l > match.Duration => PhoneDateDurMatch(lead, match, compare),
        // Lead, Comparison, and Match are all equal
        var l when l == match.Duration && l == compare.Duration => PhoneDateDurMatch(lead, match, compare),
        // Default
        _ => PhoneDateDurMatch(lead, match, compare)
    };

    internal static IDiscrepancyCall PhoneDateDurMatch(IDiscrepancyCall lead, IDiscrepancyCall match, IDiscrepancyCall comp)
    {
        if (match.Billable && !comp.Billable)
            return match;
        if (!match.Billable && comp.Billable)
            return comp;

        // Both matches have durations equal to the lead
        // We need to log this instance because this is not a good interaction and is strange 
        StringLogger.AlludeLog(
            true,
            "New log for:",
            GetFullName.GetMemberName(new MatchDiscrepancyCalls(), nameof(PhoneDateDurMatch)),
            "Three calls had the exact same datetime and duration",
            "This billed lead:", $"{lead.Number.Number}, {lead.Date}, {lead.Duration}",
            $"The current call:", $"{match.Number.Number}, {match.Date}, {match.Duration}",
            $"The closest call:", $"{comp.Number.Number}, {comp.Date}, {comp.Duration}");

        // The two have identical datetimes, identical durations, and identical billabilities, so it doesn't matter which one we return
        return match;
    }
    #endregion
}
