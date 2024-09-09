using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;

namespace Automate.Domain.DiscrepancyAnalysis;

public class AnalyzeDiscrepancyWithNotePatterns
{
    #region Public
    /// <summary>
    /// <para>This method takes the <paramref name="matches"/> of type <see cref="MatchingLeads"/> and qualifies each one and only returns those that are actually discrepancies.</para>
    /// <para>Discrepancies are defined as <see cref="MatchingLeads.BothBillable"/> is <see cref="false"/></para>
    /// </summary>
    /// <param name="matches"></param>
    /// <returns>
    /// <para><see cref="List{T}"/> where T is <see cref="DiscrepancyMatch"/></para>
    /// </returns>
    public static List<DiscrepancyMatch> FindReasoning(List<MatchingLeads> matches)
    {
        // Start log
        string location = GetFullName.GetMemberName(new AnalyzeDiscrepancyWithNotePatterns(), nameof(FindReasoning));
        StringLogger.AddLog($"Start log for {location}");

        // Calculate reasoning for all matches
        List<DiscrepancyMatch> reasoned = CalculateReasoning(matches);
        // Only return those items that are discrepancies
        List<DiscrepancyMatch> discrepancies = RetrieveDiscrepancies(reasoned);

        // Log
        StringLogger.EndAlludeLog(true, "End log of lead contents that could not be reasoned.");
        StringLogger.AddLog($"End log for {location}");

        // Return resulting discrepancies
        return discrepancies;
    }
    #endregion

    #region Internal
    /// <summary>
    /// <para>This is here for testing.</para> 
    /// <para>The count of <paramref name="matches"/> should match the count of the return <see cref="List"/>.</para>
    /// </summary>
    /// <param name="matches"></param>
    /// <returns>
    /// <para><see cref="List{T}"/> where <see cref="T"/> is <see cref="DiscrepancyMatch"/></para></returns>
    internal static List<DiscrepancyMatch> CalculateReasoning(List<MatchingLeads> matches)
    {
        // Calculate billability
        return matches
            .Select(c => new DiscrepancyMatch(c, CalculateReasoning(c)))
            .ToList();

        // Local
        static CallBillability CalculateReasoning(MatchingLeads matchingLeads)
        {
            string contents = matchingLeads.ComparisonLead.Notes;

            var result = NotesPatterns.MatchPatterns(contents);
            var matches = result.MatchedPatterns;
            
            if (result.Billability == CallBillability.Unknown)
                StringLogger.AlludeLog(true, $"{nameof(matchingLeads.ComparisonLead.Notes)} could not be reasoned", contents);
            return result.Billability;
        }
    }

    /// <summary>
    /// <para>This is here for testing.</para>
    /// <para>The resulting <see cref="List{T}"/> should contain <see cref="DiscrepancyMatch"/> where the <see cref="DiscrepancyMatch.MatchingLeads"/> are <see cref="MatchingLeads.BothBillable"/> == <see cref="true"/></para>
    /// <para>Also, the <see cref="List{T}"/> of <see cref="DiscrepancyMatch"/> does not necessarily have to have passed through the method <see cref="CalculateReasoning(List{MatchingLeads})"/></para>
    /// </summary>
    /// <param name="result"></param>
    /// <returns><see cref="List{T}"/> where T is <see cref="DiscrepancyMatch"/></returns>
    internal static List<DiscrepancyMatch> RetrieveDiscrepancies(List<DiscrepancyMatch> result)
    {
        return result.Where(r => !r.MatchingLeads.BothBillable).ToList();
    }
    #endregion
}
