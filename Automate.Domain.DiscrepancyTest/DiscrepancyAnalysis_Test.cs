using Automate.Domain.DiscrepancyAnalysis;
using Automate.Domain.ValueObjects;

namespace Automate.Domain.DiscrepancyTest;

public class DiscrepancyAnalysis_Test
{
    [
        Theory,
        InlineData(2434567890, 90, "2024-05-21 13:44:07.361+01"), // These phone numbers are random, as are the dates and duration numbers
        InlineData(8764951324, 48, "2023-06-20 14:34:26.452-08"), // These phone numbers are random, as are the dates and duration numbers
        InlineData(9467824561, 33, "2023-07-19 15:24:45.573+09"), // These phone numbers are random, as are the dates and duration numbers
        InlineData(3467258468, 12, "2023-08-18 16:14:34.689-02"), // These phone numbers are random, as are the dates and duration numbers
        InlineData(4678698520, 42, "2023-09-17 17:04:13.795+07"), // These phone numbers are random, as are the dates and duration numbers
    ]
    public void Discrepancy_MatchesCallsProperly(long number, int duration, string stringDate)
    {
        /*
         *********************************************************************************
         * ASSEMBLE
         *********************************************************************************
         */
        // Convert initial date object
        DateTime date = DateTime.Parse(stringDate);
        TimeSpan minute = TimeSpan.FromMinutes(1);
        bool billable = true;

        // Duration conversion items
        TimeSpan threeS = TimeSpan.FromSeconds(3);
        TimeSpan dur = TimeSpan.FromSeconds(duration);

        // Convert initial phone number
        const int fortyFive = 45;
        PhoneNumber phone = new(number);

        // Convert note
        string note = string.Empty;

        // Create PhoneNumber Objects
        DiscrepancyCall original = new(phone, billable, date, dur, note);

        /* 
         * Phone Number Matches
         */
        // PhoneNumberMatches AND Billable AND DateMatches    AND DurationMatches
        DiscrepancyCall phMatch_ABill_ADateA_ADurA = new(new(number), billable, date, dur, note); // These constitute the first line of each list
        // PhoneNumberMatches AND Billable AND DateMatchesPLS AND DurationMatches
        DiscrepancyCall phMatch_ABill_ADateP_ADurA = new(new(number), billable, date + threeS, dur, note);
        // PhoneNumberMatches AND Billable AND DateMatchesMNS AND DurationMatches
        DiscrepancyCall phMatch_ABill_ADateM_ADurA = new(new(number), billable, date - threeS, dur, note);
        // PhoneNumberMatches AND Billable AND DateMatches    AND DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_ADateA_ADurP = new(new(number), billable, date, dur + threeS, note);
        // PhoneNumberMatches AND Billable AND DateMatches    AND DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_ADateA_ADurM = new(new(number), billable, date, dur - threeS, note);

        // PhoneNumberMatches AND Billable AND DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_ADateP_ADurP = new(new(number), billable, date + minute, dur + threeS, note); // These constitute the second line of each list
        // PhoneNumberMatches AND Billable AND DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_ADateP_ADurM = new(new(number), billable, date + minute, dur - threeS, note);
        // PhoneNumberMatches AND Billable AND DateMatchesMIN AND DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_ADateM_ADurP = new(new(number), billable, date - minute, dur + threeS, note);
        // PhoneNumberMatches AND Billable AND DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_ADateM_ADurM = new(new(number), billable, date - minute, dur - threeS, note);

        // PhoneNumberMatches AND Billable AND DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_ADateP_NDurP = new(new(number), billable, date + minute, dur + threeS + threeS, note); // These constitute the next line of each list ...
        // PhoneNumberMatches AND Billable AND DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_ADateP_NDurM = new(new(number), billable, date + minute, dur - threeS - threeS, note);
        // PhoneNumberMatches AND Billable AND DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_ADateM_NDurP = new(new(number), billable, date - minute, dur + threeS + threeS, note);
        // PhoneNumberMatches AND Billable AND DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_ADateM_NDurM = new(new(number), billable, date - minute, dur - threeS - threeS, note);

        // PhoneNumberMatches AND Billable NOT DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_NDateP_ADurP = new(new(number), billable, date + minute + minute, dur + threeS, note); // ... and so on ... 
        // PhoneNumberMatches AND Billable NOT DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_NDateP_ADurM = new(new(number), billable, date + minute + minute, dur - threeS, note);
        // PhoneNumberMatches AND Billable NOT DateMatchesMNS AND DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_NDateM_ADurP = new(new(number), billable, date - minute - minute, dur + threeS, note);
        // PhoneNumberMatches AND Billable NOT DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_NDateM_ADurM = new(new(number), billable, date - minute - minute, dur - threeS, note);

        // PhoneNumberMatches AND Billable NOT DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_NDateP_NDurP = new(new(number), billable, date + minute + minute, dur + threeS + threeS, note);
        // PhoneNumberMatches AND Billable NOT DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_NDateP_NDurM = new(new(number), billable, date + minute + minute, dur - threeS - threeS, note);
        // PhoneNumberMatches AND Billable NOT DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_ABill_NDateM_NDurP = new(new(number), billable, date - minute - minute, dur + threeS + threeS, note);
        // PhoneNumberMatches AND Billable NOT DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_ABill_NDateM_NDurM = new(new(number), billable, date - minute - minute, dur - threeS - threeS, note);

        // PhoneNumberMatches NOT Billable AND DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_ADateP_ADurP = new(new(number), !billable, date + minute, dur + threeS, note);
        // PhoneNumberMatches NOT Billable AND DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_ADateP_ADurM = new(new(number), !billable, date + minute, dur - threeS, note);
        // PhoneNumberMatches NOT Billable AND DateMatchesMNS AND DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_ADateM_ADurP = new(new(number), !billable, date - minute, dur + threeS, note);
        // PhoneNumberMatches NOT Billable AND DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_ADateM_ADurM = new(new(number), !billable, date - minute, dur - threeS, note);

        // PhoneNumberMatches NOT Billable AND DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_ADateP_NDurP = new(new(number), !billable, date + minute, dur + threeS + threeS, note);
        // PhoneNumberMatches NOT Billable AND DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_ADateP_NDurM = new(new(number), !billable, date + minute, dur - threeS - threeS, note);
        // PhoneNumberMatches NOT Billable AND DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_ADateM_NDurP = new(new(number), !billable, date - minute, dur + threeS + threeS, note);
        // PhoneNumberMatches NOT Billable AND DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_ADateM_NDurM = new(new(number), !billable, date - minute, dur - threeS - threeS, note);

        // PhoneNumberMatches NOT Billable NOT DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_NDateP_ADurP = new(new(number), !billable, date + minute + minute, dur + threeS, note);
        // PhoneNumberMatches NOT Billable NOT DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_NDateP_ADurM = new(new(number), !billable, date + minute + minute, dur - threeS, note);
        // PhoneNumberMatches NOT Billable NOT DateMatchesMNS AND DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_NDateM_ADurP = new(new(number), !billable, date - minute - minute, dur + threeS, note);
        // PhoneNumberMatches NOT Billable NOT DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_NDateM_ADurM = new(new(number), !billable, date - minute - minute, dur - threeS, note);

        // PhoneNumberMatches NOT Billable NOT DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_NDateP_NDurP = new(new(number), !billable, date + minute + minute, dur + threeS + threeS, note);
        // PhoneNumberMatches NOT Billable NOT DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_NDateP_NDurM = new(new(number), !billable, date + minute + minute, dur - threeS - threeS, note);
        // PhoneNumberMatches NOT Billable NOT DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phMatch_NBill_NDateM_NDurP = new(new(number), !billable, date - minute - minute, dur + threeS + threeS, note);
        // PhoneNumberMatches NOT Billable NOT DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phMatch_NBill_NDateM_NDurM = new(new(number), !billable, date - minute - minute, dur - threeS - threeS, note);

        /* 
         * Phone Number doesn't match
         */
        // NOT PhoneNumberMatches AND Billable AND DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_ADateP_ADurP = new(new(number + fortyFive), billable, date + minute, dur + threeS, note);
        // NOT PhoneNumberMatches AND Billable AND DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_ADateP_ADurM = new(new(number + fortyFive), billable, date + minute, dur - threeS, note);
        // NOT PhoneNumberMatches AND Billable AND DateMatchesMIN AND DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_ADateM_ADurP = new(new(number + fortyFive), billable, date - minute, dur + threeS, note);
        // NOT PhoneNumberMatches AND Billable AND DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_ADateM_ADurM = new(new(number + fortyFive), billable, date - minute, dur - threeS, note);

        // NOT PhoneNumberMatches AND Billable AND DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_ADateP_NDurP = new(new(number + fortyFive), billable, date + minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches AND Billable AND DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_ADateP_NDurM = new(new(number + fortyFive), billable, date + minute, dur - threeS - threeS, note);
        // NOT PhoneNumberMatches AND Billable AND DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_ADateM_NDurP = new(new(number + fortyFive), billable, date - minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches AND Billable AND DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_ADateM_NDurM = new(new(number + fortyFive), billable, date - minute, dur - threeS - threeS, note);

        // NOT PhoneNumberMatches AND Billable NOT DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_NDateP_ADurP = new(new(number + fortyFive), billable, date + minute + minute, dur + threeS, note);
        // NOT PhoneNumberMatches AND Billable NOT DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_NDateP_ADurM = new(new(number + fortyFive), billable, date + minute + minute, dur - threeS, note);
        // NOT PhoneNumberMatches AND Billable NOT DateMatchesMNS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_NDateM_ADurP = new(new(number + fortyFive), billable, date - minute - minute, dur + threeS, note);
        // NOT PhoneNumberMatches AND Billable NOT DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_NDateM_ADurM = new(new(number + fortyFive), billable, date - minute - minute, dur - threeS, note);

        // NOT PhoneNumberMatches AND Billable NOT DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_NDateP_NDurP = new(new(number + fortyFive), billable, date + minute + minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches AND Billable NOT DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_NDateP_NDurM = new(new(number + fortyFive), billable, date + minute + minute, dur - threeS - threeS, note);
        // NOT PhoneNumberMatches AND Billable NOT DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_ABill_NDateM_NDurP = new(new(number + fortyFive), billable, date - minute - minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches AND Billable NOT DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_ABill_NDateM_NDurM = new(new(number + fortyFive), billable, date - minute - minute, dur - threeS - threeS, note);

        // NOT PhoneNumberMatches NOT Billable AND DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_ADateP_ADurP = new(new(number + fortyFive), !billable, date + minute, dur + threeS, note);
        // NOT PhoneNumberMatches NOT Billable AND DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_ADateP_ADurM = new(new(number + fortyFive), !billable, date + minute, dur - threeS, note);
        // NOT PhoneNumberMatches NOT Billable AND DateMatchesMNS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_ADateM_ADurP = new(new(number + fortyFive), !billable, date - minute, dur + threeS, note);
        // NOT PhoneNumberMatches NOT Billable AND DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_ADateM_ADurM = new(new(number + fortyFive), !billable, date - minute, dur - threeS, note);

        // NOT PhoneNumberMatches NOT Billable AND DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_ADateP_NDurP = new(new(number + fortyFive), !billable, date + minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches NOT Billable AND DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_ADateP_NDurM = new(new(number + fortyFive), !billable, date + minute, dur - threeS - threeS, note);
        // NOT PhoneNumberMatches NOT Billable AND DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_ADateM_NDurP = new(new(number + fortyFive), !billable, date - minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches NOT Billable AND DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_ADateM_NDurM = new(new(number + fortyFive), !billable, date - minute, dur - threeS - threeS, note);

        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesPLS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_NDateP_ADurP = new(new(number + fortyFive), !billable, date + minute + minute, dur + threeS, note);
        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesPLS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_NDateP_ADurM = new(new(number + fortyFive), !billable, date + minute + minute, dur - threeS, note);
        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesMNS AND DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_NDateM_ADurP = new(new(number + fortyFive), !billable, date - minute - minute, dur + threeS, note);
        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesMNS AND DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_NDateM_ADurM = new(new(number + fortyFive), !billable, date - minute - minute, dur - threeS, note);

        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesPLS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_NDateP_NDurP = new(new(number + fortyFive), !billable, date + minute + minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesPLS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_NDateP_NDurM = new(new(number + fortyFive), !billable, date + minute + minute, dur - threeS - threeS, note);
        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesMNS NOT DurationMatchesPLS
        DiscrepancyCall phNMatch_NBill_NDateM_NDurP = new(new(number + fortyFive), !billable, date - minute - minute, dur + threeS + threeS, note);
        // NOT PhoneNumberMatches NOT Billable NOT DateMatchesMNS NOT DurationMatchesMNS
        DiscrepancyCall phNMatch_NBill_NDateM_NDurM = new(new(number + fortyFive), !billable, date - minute - minute, dur - threeS - threeS, note);

        /*
         * Lists
         */
        // Original List
        List<DiscrepancyCall> original_List = [original];

        // No Matches
        List<DiscrepancyCall> noMatch =
            [
                phNMatch_ABill_ADateP_ADurP, phNMatch_ABill_ADateP_ADurM, phNMatch_ABill_ADateM_ADurP, phNMatch_ABill_ADateM_ADurM,
                phNMatch_ABill_ADateP_NDurP, phNMatch_ABill_ADateP_NDurM, phNMatch_ABill_ADateM_NDurP, phNMatch_ABill_ADateM_NDurM,
                phNMatch_ABill_NDateP_ADurP, phNMatch_ABill_NDateP_ADurM, phNMatch_ABill_NDateM_ADurP, phNMatch_ABill_NDateM_ADurM,
                phNMatch_ABill_NDateP_NDurP, phNMatch_ABill_NDateP_NDurM, phNMatch_ABill_NDateM_NDurP, phNMatch_ABill_NDateM_NDurM,
                phNMatch_NBill_ADateP_ADurP, phNMatch_NBill_ADateP_ADurM, phNMatch_NBill_ADateM_ADurP, phNMatch_NBill_ADateM_ADurM,
                phNMatch_NBill_ADateP_NDurP, phNMatch_NBill_ADateP_NDurM, phNMatch_NBill_ADateM_NDurP, phNMatch_NBill_ADateM_NDurM,
                phNMatch_NBill_NDateP_ADurP, phNMatch_NBill_NDateP_ADurM, phNMatch_NBill_NDateM_ADurP, phNMatch_NBill_NDateM_ADurM,
                phNMatch_NBill_NDateP_NDurP, phNMatch_NBill_NDateP_NDurM, phNMatch_NBill_NDateM_NDurP, phNMatch_NBill_NDateM_NDurM
            ];

        // Phone number matches but at least one other element does not match
        List<DiscrepancyCall> phMatchNonMatch =
            [
                phMatch_ABill_ADateP_NDurP, phMatch_ABill_ADateP_NDurM, phMatch_ABill_ADateM_NDurP, phMatch_ABill_ADateM_NDurM,
                phMatch_ABill_NDateP_ADurP, phMatch_ABill_NDateP_ADurM, phMatch_ABill_NDateM_ADurP, phMatch_ABill_NDateM_ADurM,
                phMatch_ABill_NDateP_NDurP, phMatch_ABill_NDateP_NDurM, phMatch_ABill_NDateM_NDurP, phMatch_ABill_NDateM_NDurM,
                phMatch_NBill_ADateP_ADurP, phMatch_NBill_ADateP_ADurM, phMatch_NBill_ADateM_ADurP, phMatch_NBill_ADateM_ADurM,
                phMatch_NBill_ADateP_NDurP, phMatch_NBill_ADateP_NDurM, phMatch_NBill_ADateM_NDurP, phMatch_NBill_ADateM_NDurM,
                phMatch_NBill_NDateP_ADurP, phMatch_NBill_NDateP_ADurM, phMatch_NBill_NDateM_ADurP, phMatch_NBill_NDateM_ADurM,
                phMatch_NBill_NDateP_NDurP, phMatch_NBill_NDateP_NDurM, phMatch_NBill_NDateM_NDurP, phMatch_NBill_NDateM_NDurM,
            ];

        // Reused lists
        List<DiscrepancyCall> nonMatchLists =
            [
                .. phMatchNonMatch,
                .. noMatch
            ];

        // Lists of non-exact matches
        List<DiscrepancyCall> dateP_ADurP =
            [
                phMatch_ABill_ADateP_ADurP, // Date + minute, dur + threeS
                .. nonMatchLists,
            ];
        List<DiscrepancyCall> nBill_AdateP_ADurP1 =
            [
                phMatch_ABill_ADateP_ADurM, // Date + minute, dur - threeS
                .. nonMatchLists, // The match here is the PhoneMatch, NonBillable, Date + minute, Duration + 3 seconds
            ];
        List<DiscrepancyCall> nBill_AdateP_ADurP2 =
            [
                phMatch_ABill_ADateM_ADurP, // Date - minute, dur + threeS
                .. nonMatchLists, // The match here is the PhoneMatch, NonBillable, Date + minute, Duration + 3 seconds
            ];
        List<DiscrepancyCall> nBill_AdateP_ADurP3 =
            [
                phMatch_ABill_ADateM_ADurM, // Date - minute, dur - three
                .. nonMatchLists, // The match here is the PhoneMatch, NonBillable, Date + minute, Duration + 3 seconds
            ];

        // Each of these matches the original, but they're non-exact matches
        List<DiscrepancyCall> closeMatch =
            [
                phMatch_ABill_ADateP_ADurP,
                phMatch_ABill_ADateP_ADurM,
                phMatch_ABill_ADateM_ADurP,
                phMatch_ABill_ADateM_ADurM,
            ];

        // Lists of exact matches compared against close matches and non-matches
        List<DiscrepancyCall> dateA_ADurA =
            [
                phMatch_ABill_ADateA_ADurA, // This is the exact match
                .. closeMatch,
                .. nonMatchLists,
            ];
        List<DiscrepancyCall> dateP_ADurA =
            [
                phMatch_ABill_ADateP_ADurA, // Date is plus three seconds, duration is exact match
                .. closeMatch,
                .. nonMatchLists,
            ];
        List<DiscrepancyCall> dateP_ADurP_AA = // Here, the match will be Date + minute, duration + threeS
            [
                phMatch_ABill_ADateM_ADurA, // Date - three seconds, duration is exact match
                .. closeMatch,
                .. nonMatchLists,
            ];
        List<DiscrepancyCall> dateA_ADurP =
            [
                phMatch_ABill_ADateA_ADurP, // Duration + threeS
                .. closeMatch,
                .. nonMatchLists,
            ];
        List<DiscrepancyCall> dateA_ADurM =
            [
                phMatch_ABill_ADateA_ADurM, // Duration - threeS
                .. closeMatch, // This will return the closest match of closeMatch
                .. nonMatchLists,
            ];

        /*
         *********************************************************************************
         * ACT
         *********************************************************************************
         */
        // Test the original against the other lists
        List<DiscrepancyCall> dateP_ADurP_List = [.. dateP_ADurP];
        List<MatchingLeads> dateP_ADurP_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateP_ADurP_List);
        List<DiscrepancyCall> dateP_ADurM_List = [.. nBill_AdateP_ADurP1];
        List<MatchingLeads> nBill_AdateP_ADurP1_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateP_ADurM_List);
        List<DiscrepancyCall> dateM_ADurP_List = [.. nBill_AdateP_ADurP2];
        List<MatchingLeads> nBill_AdateP_ADurP2_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateM_ADurP_List);
        List<DiscrepancyCall> dateM_ADurM_List = [.. nBill_AdateP_ADurP3];
        List<MatchingLeads> nBill_AdateP_ADurP3_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateM_ADurM_List);
        List<DiscrepancyCall> dateA_ADurA_List = [.. dateA_ADurA];
        List<MatchingLeads> dateA_ADurA_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateA_ADurA_List);
        List<DiscrepancyCall> dateP_ADurA_List = [.. dateP_ADurA];
        List<MatchingLeads> dateP_ADurA_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateP_ADurA_List);
        List<DiscrepancyCall> dateM_ADurA_List = [.. dateP_ADurP_AA];
        List<MatchingLeads> dateP_ADurP_AA_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateM_ADurA_List);
        List<DiscrepancyCall> dateA_ADurP_List = [.. dateA_ADurP];
        List<MatchingLeads> dateA_ADurP_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateA_ADurP_List);
        List<DiscrepancyCall> dateA_ADurM_List = [.. dateA_ADurM];
        List<MatchingLeads> dateA_ADurM_Original = MatchDiscrepancyCalls.MatchLeads(original_List, dateA_ADurM_List);


        /*
         *********************************************************************************
         * ASSERT
         *********************************************************************************
         */
        // Assert that the billable lead is equal to the original in every way
        // Billable Number Equals Original Number
        Assert.Equal(dateP_ADurP_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(dateA_ADurA_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(dateP_ADurA_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(dateP_ADurP_AA_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(dateA_ADurP_Original[0].BilledLead.Number.Number, original.Number.Number);
        Assert.Equal(dateA_ADurM_Original[0].BilledLead.Number.Number, original.Number.Number);
        // Billable Date Equals Original Date 
        Assert.Equal(dateP_ADurP_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(dateA_ADurA_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(dateP_ADurA_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(dateP_ADurP_AA_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(dateA_ADurP_Original[0].BilledLead.Date, original.Date);
        Assert.Equal(dateA_ADurM_Original[0].BilledLead.Date, original.Date);
        // Billable Duration equals Original Duration
        Assert.Equal(dateP_ADurP_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(dateA_ADurA_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(dateP_ADurA_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(dateP_ADurP_AA_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(dateA_ADurP_Original[0].BilledLead.Duration, original.Duration);
        Assert.Equal(dateA_ADurM_Original[0].BilledLead.Duration, original.Duration);

        // Assert that the results were found as expected
        // Assert that the numbers are the same
        Assert.Equal(dateP_ADurP_Original[0].BilledLead.Number.Number, dateP_ADurP_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].BilledLead.Number.Number, nBill_AdateP_ADurP1_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].BilledLead.Number.Number, nBill_AdateP_ADurP2_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].BilledLead.Number.Number, nBill_AdateP_ADurP3_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(dateA_ADurA_Original[0].BilledLead.Number.Number, dateA_ADurA_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(dateP_ADurA_Original[0].BilledLead.Number.Number, dateP_ADurA_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(dateP_ADurP_AA_Original[0].BilledLead.Number.Number, dateP_ADurP_AA_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(dateA_ADurP_Original[0].BilledLead.Number.Number, dateA_ADurP_Original[0].ComparisonLead.Number.Number);
        Assert.Equal(dateA_ADurM_Original[0].BilledLead.Number.Number, dateA_ADurM_Original[0].ComparisonLead.Number.Number);

        // Assert that the dates and times that should be the same are in fact the same
        Assert.Equal(dateA_ADurA_Original[0].BilledLead.Date, dateA_ADurA_Original[0].ComparisonLead.Date);
        Assert.Equal(dateA_ADurA_Original[0].BilledLead.Duration, dateA_ADurA_Original[0].ComparisonLead.Duration);
        Assert.Equal(dateP_ADurA_Original[0].BilledLead.Duration, dateP_ADurA_Original[0].ComparisonLead.Duration);
        Assert.Equal(dateP_ADurP_AA_Original[0].BilledLead.Duration, dateP_ADurP_AA_Original[0].ComparisonLead.Duration);

        // Assert that the dates and times that should be off are off by the expected amount
        Assert.Equal(dateA_ADurP_Original[0].BilledLead.Date, dateA_ADurP_Original[0].ComparisonLead.Date + minute);
        Assert.Equal(dateA_ADurM_Original[0].BilledLead.Date, dateA_ADurM_Original[0].ComparisonLead.Date + minute);
        Assert.Equal(dateP_ADurP_Original[0].BilledLead.Date, dateP_ADurP_Original[0].ComparisonLead.Date - minute);
        Assert.Equal(dateP_ADurP_Original[0].BilledLead.Duration, dateP_ADurP_Original[0].ComparisonLead.Duration - threeS);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].BilledLead.Date, nBill_AdateP_ADurP1_Original[0].ComparisonLead.Date + minute);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].BilledLead.Duration, nBill_AdateP_ADurP1_Original[0].ComparisonLead.Duration - threeS);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].BilledLead.Date, nBill_AdateP_ADurP2_Original[0].ComparisonLead.Date + minute);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].BilledLead.Duration, nBill_AdateP_ADurP2_Original[0].ComparisonLead.Duration - threeS);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].BilledLead.Date, nBill_AdateP_ADurP3_Original[0].ComparisonLead.Date + minute);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].BilledLead.Duration, nBill_AdateP_ADurP3_Original[0].ComparisonLead.Duration - threeS);

        // Assert that the comparison lead is the correct one
        Assert.Equal(dateP_ADurP_Original[0].ComparisonLead.Number.Number, dateP_ADurP[0].Number.Number);
        Assert.Equal(nBill_AdateP_ADurP1_Original[0].ComparisonLead.Number.Number, nBill_AdateP_ADurP1[0].Number.Number);
        Assert.Equal(nBill_AdateP_ADurP2_Original[0].ComparisonLead.Number.Number, nBill_AdateP_ADurP2[0].Number.Number);
        Assert.Equal(nBill_AdateP_ADurP3_Original[0].ComparisonLead.Number.Number, nBill_AdateP_ADurP3[0].Number.Number);
        Assert.Equal(dateA_ADurA_Original[0].ComparisonLead.Number.Number, dateA_ADurA[0].Number.Number);
        Assert.Equal(dateP_ADurA_Original[0].ComparisonLead.Number.Number, dateP_ADurA[0].Number.Number);
        Assert.Equal(dateP_ADurP_AA_Original[0].ComparisonLead.Number.Number, dateP_ADurP_AA[0].Number.Number);
        Assert.Equal(dateA_ADurP_Original[0].ComparisonLead.Number.Number, dateA_ADurP[0].Number.Number);
        Assert.Equal(dateA_ADurM_Original[0].ComparisonLead.Number.Number, dateA_ADurM[0].Number.Number);

        // Assert that none of the results are blank or empty
        Assert.NotEqual(dateP_ADurP_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(nBill_AdateP_ADurP1_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(nBill_AdateP_ADurP2_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(nBill_AdateP_ADurP3_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(dateA_ADurA_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(dateP_ADurA_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(dateP_ADurP_AA_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(dateA_ADurP_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
        Assert.NotEqual(dateA_ADurM_Original[0].ComparisonLead.Number.Number, MatchDiscrepancyCalls._defaultCall.Number.Number);
    }
}
