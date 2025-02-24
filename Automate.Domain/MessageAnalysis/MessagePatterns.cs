using Automate.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Automate.Domain.MessageAnalysis;

internal record ClassificationResult(string Matches, string Input, bool Result, bool NoMatches);

internal partial class MessagePatterns
{
    #region Internal
    internal const string Match = "Matched Pattern:";
    internal readonly static string Customer1Name = nameof(Customer1);
    internal readonly static string Customer2Name = nameof(Customer2);
    internal readonly static string NotTreatedName = nameof(NotTreated);
    internal readonly static string ReferralName = nameof(Referral);
    internal readonly static string PossibleName = nameof(Possible);
    internal readonly static string LikelyName = nameof(Likely);
    internal static ClassificationResult Billable(string input)
    {
        Match customerMatch =
            Customer1().Match(input);
        Match customer2Match =
            Customer2().Match(input);
        Match notTreatedMatch =
            NotTreated().Match(input);
        Match referralMatch =
            Referral().Match(input);
        Match possibleMatch =
            Possible().Match(input);
        Match likelyMatch =
            Likely().Match(input);

        List<string> message = new(6);
        if (customerMatch.Success)
            message.Add($"{Match} {Customer1Name} => ({customerMatch.Value})");
        if (customer2Match.Success)
            message.Add($"{Match} {Customer2Name} => ({customer2Match.Value})");
        if (notTreatedMatch.Success)
            message.Add($"{Match} {NotTreatedName} => ({notTreatedMatch.Value}");
        if (referralMatch.Success)
            message.Add($"{Match} {ReferralName} => ({referralMatch.Value})");
        if (possibleMatch.Success)
            message.Add($"{Match} {PossibleName} => ({possibleMatch.Value})");
        if (likelyMatch.Success)
            message.Add($"{Match} {LikelyName} => ({likelyMatch.Value})");

        const string bar = " | ";
        string matches = message.Count == 0 ? string.Empty : string.Join(bar, message);

        /* I know what you're asking:
         * "Why are you using so many patterns when you seem to only care about two?"
         * Well first of all, order matters here
         * Also, we want to know which pattern matched the text, of course
         */
        bool result = false;
        if (customerMatch.Success || customer2Match.Success || notTreatedMatch.Success || referralMatch.Success)
            result = false;
        else if (likelyMatch.Success || possibleMatch.Success)
            result = true;

        return new(matches, input, result, matches == string.Empty);
    }
    #endregion

    #region Generated Regex List
    [GeneratedRegex(_customer1, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex Customer1();
    [GeneratedRegex(_customer2, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    public static partial Regex Customer2();
    [GeneratedRegex(_referral, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex Referral();
    [GeneratedRegex(_notTreated, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex NotTreated();
    [GeneratedRegex(_likely, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex Likely();
    [GeneratedRegex(_possible, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex Possible();
    #endregion

    #region Generalized Strings
    private const string border = @"\b";
    private const string chr = ".";
    private const string chars = $"({chr})*";
    private const string empty = @"(\s|\n\r|\r\n|\n|" + orPipe + @"|\r)*";
    private const string letters = $"({ltr})*";
    private const string ltr = @"(\w)";
    private const string orPipe = @"\|";
    private const string qMark = @"\?";
    private const string word = $"({ltr}+ )";
    private const string words = $"({ltr}+ )*";
    private const string zeroThree = "{0,3}";
    private const string zeroFive = "{0,5}";

    private const string appointment = $"((ap{letters}t){letters})";
    private const string cancel = $"{border}(end|stop|cancel{letters})";
    private const string companyName = PatternHelper.Company;
    private const string customerService = $"({customer} service)";
    private const string home = $"((home|house|place|resid){letters})";
    private const string nest = $"((nest){letters})";
    private const string me = $"(me|my)";
    private const string my = $"((mine|our|{me}){letters})";
    private const string product = $"((product|chemical|spray){letters})";
    private const string preposition = $"(in|around|near|beside|between|by){letters}";
    private const string quote = $"((pric|{qote}|estimat|how much|eval|cost){letters})";
    private const string service = $"((problem|remov|re{letters}c{letters}ur{letters} servic|treat|spray|bomb|(pest|yard|home) control|visit|servi(c|d)|exterminat|infest|inspect|fumiga|{quote}){letters})";
    private const string toBe = $"(is|are|was|were|be{letters})";
    private const string want = $"((busc|quier|{word}?want|need|{word}{zeroFive}interes{chars}{word}{zeroThree}|try{letters} to |lik{letters} ?{word}{zeroThree}|would like|look{letters} ?{word}{zeroThree}|request|provid|desir){letters}{border})";
    private const string we = $"((we|us|{me}){letters}|i)";
    private const string weAre = $"(i{chars}m ?{letters}|we({chars}re))";
    private const string work = $"((work|tra(b|v)a(j|l)|emple|hir|job|interview|employ){letters}{border})";
    #endregion

    #region Bug String
    internal const string bug =
    "((" +
    $"{border}ant|" +
    "bug|" +
    "bee|" +
    "beetle|" +
    $"box{chars}elder|" +
    $"carpenter{chars}(ant|bee)|" +
    "centipede|" +
    $"(c{chars}k)?{chars}ro{chars}ch|" +
    "cricket|" +
    "earwig|" +
    "fle|" +
    "fly|" +
    "fli|" +
    $"{border}(g)?nat{border}|" +
    "hornet|" +
    "insect|" +
    $"lady{chr}{zeroFive}bug|" +
    "mice|" +
    "millipede|" +
    "mole|" +
    $"mosqu{letters}o|" +
    "moth|" +
    "mouse|" +
    $"{nest}|" +
    $"{border}pest{chars}(control)?|" +
    $"{border}rat|" +
    "rodent|" +
    $"scorp{letters}n|" +
    $"{service} request|request {service}|" +
    $"silver{chars}fish|" +
    "spider|" +
    "spray|" +
    $"stink{chars}bug|" +
    "termite|" +
    "tic|" +
    "wasp|" +
    $"yello{chars}jacket{chars}|" +
    // end
    $"{service}" +
    $"){letters})";
    #endregion

    #region Customer String
    internal const string customer =
    "(" +
    // Client
    "client|clietn|cliten|cltien|ctlien|tclien|" +
    "clinet|clniet|cnliet|ncliet|" +
    "cleint|celint|eclint|clinte|" +
    "cilent|iclent|clenti|clenit|" +
    "lcient|cientl|cienlt|cielnt|" +
    "licent|liecnt|lienct|lientc|" +
    // Missing a t
    "clien|cline|clnie|cnlie|nclie|" +
    "clein|celin|eclin|" +
    "cilen|iclen|cleni|" +
    "lcien|cienl|cieln|" +
    "licen|liecn|lienc|" +
    // Missing an n
    "cliet|clite|cltie|ctlie|tclie|" +
    "cleit|celit|eclit|" +
    "cilet|iclet|cleti|" +
    "lciet|cietl|cielt|" +
    "licet|liect|lietc|" +
    // Missing an e
    "clint|clitn|cltin|ctlin|tclin|" +
    "clnit|cnlit|nclit|" +
    "cilnt|iclnt|clnti|" +
    "lcint|cintl|cinlt|" +
    "licnt|linct|lintc|" +
    // Missing an i
    "clent|cletn|clten|ctlen|tclen|" +
    "clnet|cnlet|nclet|" +
    "celnt|eclnt|clnte|" +
    "lcent|centl|cenlt|" +
    "lecnt|lenct|lentc|" +
    // Missing an l
    "cient|cietn|citen|ctien|tcien|" +
    "cinet|cniet|nciet|" +
    "ceint|ecint|cinte|" +
    "icent|centi|cenit|" +
    "iecnt|ienct|ientc|" +

    // Customer
    "customer|customre|custorme|custrome|cusrtome|curstome|crustome|rcustome|" +
    "custoemr|custeomr|cusetomr|cuestomr|ceustomr|ecustomr|" +
    "custmoer|cusmtoer|cumstoer|cmustoer|mcustoer|custoerm|" +
    "cutsomer|ctusomer|tcusomer|cusomert|cusometr|cusomter|cusotmer|" +
    "ucstomer|usctomer|ustcomer|ustocmer|ustomcer|ustomecr|ustomerc|" +
    // Customer missing an r
    "custome|custoem|custeom|cusetom|cuestom|ceustom|ecustom|" +
    "custmoe|cusmtoe|cumstoe|cmustoe|mcustoe|" +
    "cusotme|cuostme|coustme|ocustme|custmeo|" +
    "cutsome|ctusome|tcusome|cusomet|cusomte|" +
    "csutome|scutome|cutomes|cutomse|cutosme|" +
    "ucstome|cstomeu|cstomue|cstoume|cstuome|" +

    // Custoner with an n instead of m
    "custoner|custonre|custorne|custrone|cusrtone|curstone|crustone|rcustone|" +
    "custoenr|custeonr|cusetonr|cuestonr|ceustonr|ecustonr|" +
    "custnoer|cusntoer|cunstoer|cnustoer|ncustoer|custoern|" +
    "cutsoner|ctusoner|tcusoner|cusonert|cusonetr|cusonter|cusotner|" +
    "ucstoner|usctoner|ustconer|ustocner|ustoncer|ustonecr|ustonerc|" +
    // Custoner with an n nissing an r
    "custone|custoen|custeon|cuseton|cueston|ceuston|ecuston|" +
    "custnoe|cusntoe|cunstoe|cnustoe|ncustoe|" +
    "cusotne|cuostne|coustne|ocustne|custneo|" +
    "cutsone|ctusone|tcusone|cusonet|cusonte|" +
    "csutone|scutone|cutones|cutonse|cutosne|" +
    "ucstone|cstoneu|cstonue|cstoune|cstuone|" +

    // end
    "cus" +
    ")";
    #endregion

    #region Quote String
    internal const string qote =
    "(" +
    // Quote
    "quote|quoet|queot|qeuot|equot|" +
    "qutoe|qtuoe|tquoe|" +
    "qoute|oqute|quteo|" +
    "uqote|qoteu|qotue|" +
    "uoteq|uotqe|uoqte|" +
    // Missing an e
    "quot|quto|qtuo|tquo|" +
    "qout|oqut|" +
    "uqot|qotu|" +
    "uotq|uoqt|" +
    // Missing an o
    "qute|quet|qeut|equt|" +
    "qtue|tque|" +
    "uqte|qteu|" +
    "uteq|utqe|" +
    // Missing a t
    "quoe|queo|qeuo|equo|" +
    "qoue|oque|" +
    "uqoe|qoeu|" +
    "uoeq|uoqe|" +

    // Using a c instead of a q
    "cuote|cuoet|cueot|ceuot|ecuot|" +
    "cutoe|ctuoe|tcuoe|" +
    "coute|ocute|cuteo|" +
    "ucote|coteu|cotue|" +
    "uotec|uotce|uocte|" +
    // Missing an e
    "cuot|cuto|ctuo|tcuo|" +
    "cout|ocut|" +
    "ucot|cotu|" +
    "uotc|uoct|" +
    // Missing an o
    "cute|cuet|ceut|ecut|" +
    "ctue|tcue|" +
    "ucte|cteu|" +
    "utec|utce|" +
    // Missing a t
    "cuoe|cueo|ceuo|ecuo|" +
    "coue|ocue|" +
    "ucoe|coeu|" +

    // end
    "uoec|uoce" + // end does not shave an or pipe "|"
    ")";
    #endregion

    #region Customer 1
    private const string _customer1 =
    $"^{empty}(this is a )?test{letters}{empty}$|" +
    $"(retreat|restart){letters}|" +
    $"see{letters} {words}again|" +
    $"reschedul{letters}( {service})?|" +
    $"(current|exist|already|{weAre}){letters} {words}{customer}|" +
    $"{companyName} (pest control|{service}){letters}{border}|" +
    $"(am|i|are|we) {words}schedul{letters} (for{chars}day|{chars}day|tom{chr}o{chr}row|next {letters})|" +
    $"{we}{chars} {words}({service}|you) {words}(before|in the past|last {letters}|again)|" +
    $"(my|our|the|your) next {word}{zeroThree}{service}|" +
    $"(you|{companyName}|your) (are|will|have) {service}|" +
    $"{customer} portal|" +
    $"update{letters} {words}(on|concern|about){letters} {words}{service}|" +
    $"(they|s?he){letters} (is|work|represent){letters} (employed by|for|with|{word} behalf of) (you|{companyName})|" +
    $"sign{chars}up {word}{zeroFive}({service}|you|you guys|{companyName})|" +
    $"prepar{letters}|" +
    $"{weAre} {word}{zeroFive}{my} contract|" +
    $"(contract|{service}){word}{zeroFive}{cancel}|" +
    $"{cancel}{chars}(contract|{service})|" +
    $"(resum|continu){letters} {word}{zeroThree}{service}|" +
    $"{cancel}{letters}|" +
    $"{we} {words}(charg|debit){letters}|" +
    $"be{letters} (in|on) (hold|the phone|a call) with|" +
    $"you{letters} (com|cam|visit){letters}|" +
    $"(you|tech|s?he|they){chars} (cam|com){letters}|" +
    $"(change|{my}) {word}{zeroThree}(pay{letters}|credit card|cc|card)|" +
    $"pay{letters} {word}(bill|bil|contract|charge)|" +
    $"(we|i) {word}{zeroThree}(a tech|sign{letters}{chr}up|have {word}acc(oun)?t|have {word}({service}|contract))|" +
    $"(next|upcom|new){letters} {service}|" +
    $"(email|contract) {word}{zeroThree}sent( to {letters})?|" +
    $"{my}? acc(oun)?t( num{letters})?|" +
    $"({want} {words})?{work}|" +
    $"schedul{letters} {service}|" +
    $"{service} schedul{letters}|" +
    $"(send|call|talk to|get a hold of|when) {words}tech{letters}{border}|" +
    $"(you{letters}|{my}|the) call{letters}{border}|" +
    $"{service} (in|out)side|" +
    // end
    "(bill|statement|invoice)";
    #endregion

    #region Customer 2
    private const string _customer2 =
    $"(you |we had {word}{zeroThree})(treat|servic|spray){letters}|" +
    $"(when|what time) {word}{zeroThree}(appoint|treat|spray|servic){letters}|" +
    $"call{letters} {word}{zeroThree}back|" +
    $"follow{chars}up|" +
    $"(spok|speak|talk|text|email){letters} {word}{zeroThree}(earlier|early|before|.*day)|" +
    $"(spray|treat){letters} {words}again|" +
    $"you{letters} (spray|treat|servic){letters} {words}(before|in the past|again|.*day)|" +
    $"(regular{letters}|another) {words}(schedul|servic|treat|tech|visit){letters}|" +
    $"(is|are) {word}{zeroFive}(com|cam){letters}|" +
    $"still {word}{word}{zeroThree}{bug}|" +
    $"({border}{service}|tech{letters}) (was|were)|" +
    $"still {words}schedul{letters}|" +
    $"sign{letters} {words}(contract|you|{service}|company|{companyName})|" +
    $"call{letters} {words}(you|him|her|them|{companyName})|" +
    $"renew{letters} {words}({service}|plan|treat){letters}|" +
    $"tech{letters} {words}(said|rec{letters}o{letters}mend|{want}|treat|driv|drove|did|was|suppose){letters}|" +
    $"{border}{service} {word}already|" +
    $"set up (more traps?|{service})|" +
    $"next pay{letters}|" +
    $"(your|{companyName}|this) {words}company|" +
    $"(continu|follow{chr}up){letters} {words}({service}|schedule|appointment)|" +
    $"(pay|make) {word}{zeroThree}bill((.pay|charg){letters})?|" +
    $"(send|have) ?{word}{zeroThree}(some|tech){letters}|" +
    $"in {words}system|" +
    $"({border}{my}|{companyName}) (acc(oun)?t|plan|{service}|appoint|tech|system){letters}|" +
    $"(your|{companyName}) {words}system|" +
    // end
    $"cancel{letters} ?{words}{service}?";
    #endregion

    #region Referral
    private const string _referral =
    $"refer{letters}|" +
    $"{my} neighbor{chars} {words}{service}|" +
    $"s(aw|ee) (the|your|{companyName}){chars} (truck|car|pickup|rep){letters}|" +
    // end
    $"re{letters}com{letters}end{letters}";
    #endregion

    #region Not Treated
    private const string _notTreated =
    $"((" +
    $"honey{chars}bee|" +
    $"wild{chars}life|" +
    $"({want} )?{work}|" +
    $"bury{letters} {words}urn" +
    $"bee{chr}keep|" +
    $"gopher|" +
    $"opossum|" +
    $"possum|" +
    $"squirrel|" +
    $"snake|" +
    $"chipm{letters}nk|" +
    $"ground{chars}hog|" +
    $"wood{chars}chuck|" +
    $"fox|" +
    $"bird|" +
    $"raccoon|" +
    // end
    $"skunk)" +
    $"{letters})";
    #endregion

    #region Possible Lead
    private const string _possible =
    $"(larg|hug|vast|enorm){chars}{nest}|" +
    $"{bug}( {service}| {nest})?|" +
    $"remov{letters}|" +
    $"{service}|" +
    $"{want} bait|" +
    $"{quote}|" +
    $"({service}|{nest}) request{letters}|" +
    $"bit{letters} mark{letters}|" +
    // end
    $"exterminat{letters}";
    #endregion

    #region Likely
    private const string zip = @"\d{5}"
        ;
    private const string l =
    $"^{empty}$|" +
    $"^est{letters} interesa{letters}$|" +
    $"^urgent need$|" +
    "^something.else...$|" +
    $"^{empty}emergency{empty}$|" +
    $"^{empty}help{empty}$|" +
    $"^{empty}(hi|hello|hola){empty}$|" +
    $"^{empty}info{letters}{empty}$|" +
    $"^{empty}need {word}{zeroFive}{service}{empty}$|" +
    $"^{empty}{quote}{empty}$|" +
    $"^{empty}{zip}{empty}$|" +
    $"{PatternHelper.Name}|" //+
        ;
    private const string li =
    $"{toBe} {word}open .*(day|mor{chars}ow|week)|" +
    $"where (are you|is your) (locat|offic){letters}|" +
    $"{toBe} you{letters} (locat|offic){letters} {preposition}|" +
    $"do you serv{letters} {words}area|" +
    $"{empty}service{empty}|" +
    $"get{letters} {words}{home} {service}|" +
    $"hear{letters} {words}activity|" +
    $"contact (me|us|asap)|" +
    $"you ({service}|do) apartment{letters}|" +
    $"(do you|you guys|y{chars}all) {words}(work in|{service})|" +
    $"{want} {words}(agent|tech){letters}|" +
    $"(would|will|can) {words}come out|" +
    $"what (is|are|was|were|be){letters} {words}({service} )?(fee|charg){letters}( {words}{service})?|" +
    $"{want} {words}(fee|charg){letters}|" //+
        ;
    private const string lik =
    $"(giv|offer){letters} {words}{quote}|" +
    $"one{chars}time {service}|" +
    $"coupon{letters}|" +
    $"how much {words}({quote}|it is|{service})|" +
    $"(what|how much) {words}{quote}|" +
    $"consult{letters}|" +
    $"(something|{bug}) in{letters} {words}(wall|ceil|crawl{chars}space){letters}|" +
    $"do you serv{letters} {words}{qMark}|" +
    $"new {customer}|" +
    $"{want} (help|{service})|" +
    $"(fre|discount){letters} ({service}|{quote})|" +
    $"(same{chars}day|you|do you) {words}{service}|" +
    $"(you|the){chars} {product} {words}{chars}(safe|friendly){border}|" +
    $"do{letters} {words}{service}|" +
    $"({want}|make|trying to see) {words}({appointment}|{service})|" //+
        ;

    private const string _likely =
        l + li + lik +
    $"({want} )?{words}({service}|{appointment})";
    #endregion
}