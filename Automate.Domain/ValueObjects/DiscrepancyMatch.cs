namespace Automate.Domain.ValueObjects;

public class DiscrepancyMatch(MatchingLeads matchingLeads, CallBillability reasoning)
{
    public MatchingLeads MatchingLeads { get; set; } = matchingLeads;
    public CallBillability Reasoning { get; set; } = reasoning;
    private string? reason;
    public string ReasoningStr
    {
        get
        {
            if (reason is null)
            {
                string localReason = Reasoning.ToString();

                // Iterate through each character, except the first, and add a space before it
                List<char> chars = [];
                for (var i = 0; i < localReason.Length; i++)
                {
                    if (char.IsUpper(localReason[i]) && i != 0)
                    {
                        chars.Add(' ');
                    }
                    chars.Add(localReason[i]);
                }

                // Reassemble the item into a string
                reason = string.Join("", chars);
            }
            return reason;
        }
    }

}