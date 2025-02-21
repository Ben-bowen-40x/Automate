namespace Automate.Domain.ValueObjects;

public class DiscrepancyMatch(IMatchingLeads matchingLeads, CallBillability reasoning)
{
    public IMatchingLeads MatchingLeads { get; set; } = matchingLeads;
    public CallBillability Reasoning { get; set; } = reasoning;
    private string? _reason;
    public string ReasoningStr
    {
        get
        {
            if (_reason is null)
            {
                string localReason = Reasoning.ToString();

                // Iterate through each character, except the first, and add a space before it if it's capitalized
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
                _reason = string.Join(string.Empty, chars);
            }
            return _reason;
        }
    }

}