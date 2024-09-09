using System.Text.RegularExpressions;
using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;

namespace Automate.Domain.DiscrepancyAnalysis;
public record NotesPatternMatches(CallBillability Billability, string MatchedPatterns);

public partial class NotesPatterns
{
    #region Public
    public static NotesPatternMatches MatchPatterns(string input)
    {
        // Ensure that the order of the list is exactly as expected
        // The number of patterns here should match the number of patterns in the dictionary
        CallBillability[] billArr =
        [
            // 1
            CallBillability.Referral,
            // 2
            CallBillability.Spanish,
            // 3
            CallBillability.WrongArea,
            // 4
            CallBillability.ServiceNotOffered,
            // 5
            CallBillability.Renter,
            // 6
            CallBillability.CurrentCustomer,
            // 7
            CallBillability.MarkedIncorrectly,
            // 8
            CallBillability.RepeatCaller,
            // 9
            CallBillability.HangUp,
            // 10
            CallBillability.MissedCall,
            // 11
            CallBillability.Unknown,
        ];

        // Execution
        Match forNeighbor = ForNeighbor().Match(input);
        Match referral = Referral().Match(input);
        Match repeat = RepeatCaller().Match(input);
        Match spanish = Spanish().Match(input);
        Match wrongArea = WrongArea().Match(input);
        Match serviceNotOffered = ServiceNotOffered().Match(input);
        Match renter = Renter().Match(input);
        Match customer = CurrentCustomer().Match(input);
        Match missedCall = MissedCall().Match(input);
        Match hangUp = HangUp().Match(input);
        Match markedIncorrectly = MarkedIncorrectly().Match(input);

        // Match against patterns
        // The number of patterns here should match the number of patterns in the array
        Dictionary<CallBillability, Match> matchList = new()
        {
            // 1
            { CallBillability.Referral, referral },
            // 2
            { CallBillability.RepeatCaller, repeat},
            // 3
            { CallBillability.Spanish, spanish},
            // 4
            { CallBillability.WrongArea, wrongArea},
            // 5
            { CallBillability.ServiceNotOffered, serviceNotOffered},
            // 6
            { CallBillability.Renter, renter},
            // 7
            { CallBillability.CurrentCustomer, customer},
            // 8
            { CallBillability.HangUp, hangUp},
            // 9
            { CallBillability.MarkedIncorrectly, markedIncorrectly},
            // 10
            { CallBillability.MissedCall, missedCall},
            // 11
            { CallBillability.Unknown, forNeighbor },
        };

        // Set up billability and matches
        List<string> matches = [];
        CallBillability billability = CallBillability.Unknown;
        bool keepGoing = true;
        for (var i = 0; i < billArr.Length; i++)
        {
            if (matchList[billArr[i]].Success)
            {
                matches.Add($"Pattern Name: {billArr[i]} => ({matchList[billArr[i]].Value})");
                if (keepGoing)
                {
                    // Ensure that if a caller wants {MessagePatternHelper.CompanyType} for their neighbor, then we don't know how to mark this call because it was billed and it's also really not serviceable                    
                    if (matchList[billArr[i]] == matchList[CallBillability.MarkedIncorrectly] && matchList[CallBillability.Unknown].Success)
                        billability = CallBillability.Unknown;
                    else
                        billability = billArr[i];
                    keepGoing = false;
                }
            }
        }

        // Assemble matched patterns
        string matchedPatterns = string.Join(_delimiter, matches);

        return new NotesPatternMatches(billability, matchedPatterns);
    }
    #endregion

    #region Generated Expressions
    // Private members
    private const string _delimiter = " | ";
    // 1
    [GeneratedRegex(_markedIncorrectly, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex MarkedIncorrectly();
    // 2
    [GeneratedRegex(_serviceNotOffered, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex ServiceNotOffered();
    // 3
    [GeneratedRegex(_wrongArea, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex WrongArea();
    // 4
    [GeneratedRegex(_apartmentCaller, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex Renter();
    // 5
    [GeneratedRegex(_currentCustomer, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CurrentCustomer();
    // 6
    [GeneratedRegex(_missedCall, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex MissedCall();
    // 7
    [GeneratedRegex(_hangUp, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex HangUp();
    // 8
    [GeneratedRegex(_spanish, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex Spanish();
    // 9
    [GeneratedRegex(_repeat, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex RepeatCaller();
    // 10
    [GeneratedRegex(_referral, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex Referral();
    // 11
    [GeneratedRegex(_markedCorrectly, RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex ForNeighbor();
    #endregion

    #region Pattern Helpers
    private const string letters = @"(\w)*";
    private const string chr = ".";
    private const string chars = $"({chr})*";
    private const string boundary = @"\b";
    private const string word = @"(\w+ )";
    private const string words = $"({word})*";
    private const string zeroFive = "{0,5}";
    private const string zeroThree = "{0,3}";
    private const string company = PatternHelper.Company;
    private const string their = $"(their|the|{company}{chars}{letters}|the {company}|our)";
    private const string they = $"(they|{company}|we|he|she|them|him|her)";
    private const string rep = $"(rep|sale{chars}(man|woman|person|guy)|{they}){letters}";
    private const string estimate = $"((estimat|inspect|{quote}){letters}{boundary})";
    private const string service = $"((plan|servic|treat|{estimate}|app{chars}t|estimat|take care of ?){letters}{boundary})";
    private const string request = $"(look{letters} ?|need|ask{letters} ?|want|request){letters}{boundary}";
    private const string said = $"((say|said|stat|intimat|allud){letters}{boundary})";
    private const string can = $"((could|can|will|would|do{letters}){boundary})";
    private const string recommend = $"(({said} {they} {can} {offer}|rec{chars}om{chars}end|assur{letters} {word}{zeroFive}{they}){letters}{boundary})";
    private const string not = $"(not|(is|are|do|ca){chars}n{chars}t(.)?)";
    private const string offer = $"((offer|provid|treat|servic|cover|do|spray){letters}{boundary})";
    private const string ntPest = $"(((honey|bumble){chars}bee|snake|gopher|ground{chars}hog|squirrel|wildlife){letters}{boundary})";
    private const string area = $"(((zip|area|region|city)({chars}code)?|address|state|neighborhood){letters}{boundary})";
    private const string wrong = @"(wrong|incorrect|improper|outside( \w+){0,3}\b|not in\w*( of)?\b)"; // This cannot use string interpolation
    private const string b = $"(unit|complex|building|condo{letters}|apartment)";
    private const string apartment = $"({b}|apartment(.)?{b})";
    private const string o = $"((own|manag|admin){letters}{boundary})";
    private const string manager = $"(((land|propert|ho(m|us)e|{apartment}) ?{o})|{o})";
    private const string hangup = $"(h.ng(s|ed|ing)?{chars}up)";
    private const string tech = $"((tech|employee|truck|advert){letters}{boundary})";
    private const string quote = $"({MessagePatterns.qote}{letters})";
    private const string talkTo = $"((reach{letters} out to|talk{letters} to|speak{letters}( ?(to|with))?|get in touch with|call|permission from|contact|coordinat|check with){letters}{boundary})";
    private const string many = "(multiple|many|several|a lot( of)?|a bunch( of)?)";
    #endregion

    #region Patterns
    private const string _markedIncorrectly =
        @"\$.?\d+.*(\$.?\d+)?|" +
        $"problem.*request{letters} {word}call back|" +
        $"{request} {PatternHelper.CompanyType} {service}{letters} for {letters}{boundary}|" +
        $"same(.)?day {word}{zeroFive}{service}|" +
        $"(book|schedule|set up|get) {word}{zeroThree}{service}|" +
        $"{request} {words}free {estimate}|" +
        $"{rep} ({recommend}|{said}) {words}{service}|" +
        $"{said} {words}{talkTo} {words}{they} (soon|as soon as|right away|immediate{letters})|" +
        $"{request} {PatternHelper.CompanyType} ({service}|company)|" +
        $"{request} {words}a {quote}|" +
        $"{rep} {said} {words}{service}|" +
        $"{service} (was|is|were|are) (schedule|set up|set)|" +
        $"{rep} {recommend} {their} {word}{zeroFive}{service}|" +
        $"({rep}|caller) ({said} )?{they} (will|would|should|shall) {word}{zeroThree}call {word}{zeroThree}back|" +
        $"{rep} {recommend} {word}{zeroThree}{their} {service}|" +

        // Additional Patterns
        $"conversation.*(inquir|service agreement|suspect){letters} {words}{MessagePatterns.bug}|" +
        $"call(ed)?.*(inquir|\\w*ing (\\w* )*help|request|suspect|issue||quote|address|request|about|get\\w*( rid of )?|report)(\\w* )*{MessagePatterns.bug}";

    private const string _markedCorrectly =
        $"for {their} neighbor";

    private const string _serviceNotOffered =
       $"{ntPest}|" +
       $"no.kill {word}{zeroThree}for|" +
       $"{they} {words} {not} {offer}( {word}{zeroThree}{service})?|" +
       $"{service} {not} {offer}";

    private const string _wrongArea =
        $"{they} {not} ({offer} )?({service} )?{word}{zeroFive}{area}|" +
        $"{wrong} ({service} )?{area}|" +
        $"{they} {not} {word}{zeroThree}{area}";

    private const string _apartmentCaller =
       $"(single|attached) {apartment}( in a complex)?|" +
       $"{apartment} caller|" +
       $"(discuss|talk|ask) {word}{zeroFive}{manager}|" +
       $"({apartment} )?{manager} (permission|approval)|" +
       $"{talkTo} {manager}|" +
       $"{not} {offer} {word}{zeroThree}{apartment}|" +
       $"(need|ask|get){letters} {words}{manager}( permission)?";

    private const string _currentCustomer =
       $"((caller|this|we|i)? ?(is |am )a )?current {MessagePatterns.customer}|" +
       "previously billed";

    private const string manyDW = @"(\d+|\w+)";
    private const string _missedCall =
       $"no one {word}{zeroThree}after {words}{manyDW} (second|minute)|" +
       $"on hold {word}{zeroFive}|" +
       $"(caller )?interact{letters} {word}{zeroThree}ivr|" +
       $"caller {hangup} before|" +
       "(hold|wait) (music|time)|" +
       $"call (rings|rang) {many}|" +
       $"({hangup} (after|immediately)|(caller )?(rings|rang) {many} time(s)?)|" +
       "code red|" +
       $"{hangup} without (leaving )?(a |any )?message|" +
       $"call end{letters} after|" +
       $"{hangup} before (anyone|someone) (can|could) answer|" +
       $"{hangup} without interact{letters} {words}ivr|" +
       @"(answering )?machine|" +

       // Additional
       $"conversation {words}(record{letters} message|automated)";

    private const string _hangUp =
       $"call {word}{zeroFive}disconnect{letters}{boundary}|" +
       $"no one speaks {words}(line|end)|" +
       "with a live agent|" +
       $"{hangup} on {words}{rep}|" +
       $"(call|speak){letters} {hangup} {words}call {words}answer{letters}{boundary}|" +
       $"(end|{hangup}){letters} {word}{zeroThree}immediat{letters}|" +
       "dead air|" +
       "no audio|" +
       "(unresponsive caller|caller (is |was )?unresponsive)|" +
       $"caller {hangup} up {words}{rep} answer{letters}{boundary}( the call)?";

    private const string _spanish =
        "spanish( call| speaking)?|" +
        "call is in spanish";

    private const string _repeat =
        $"caller {words}(return{letters} call|call{letters} back)";

    private const string _referral =
        $"{tech} {words}in {words}{area}|" +
        $"caller {words}refer{letters}{boundary}";
    #endregion
}
