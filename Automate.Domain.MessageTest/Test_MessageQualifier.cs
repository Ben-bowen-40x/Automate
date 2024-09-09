using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
namespace Automate.Domain.TextQualificationTests;

public class Test_MessageQualifier
{
    [
        Theory,
        InlineData(
        /*PhoneNumber, (phony data)*/ 13456789012,
        /*Contents, (real data)*/ "I need someone to come and spray for ants.In my kitchen and are pretty bad.What does it cost and how soon can someone come?",
        /*Date, (real date)*/ "2024-05-22 20:22:19+06:00",
        /*Call Billable*/ true,
        /*Text Billable*/ true),
    //InlineData(
    ///*PhoneNumber, (phony data)*/ 12345678901,
    ///*Contents, (real data)*/ "Hello - i just want to confirm that I should still be expecting treatment today",
    ///*Date, (real date)*/ "2024-05-22 19:20:47+06:00",
    ///*Call Billable*/ true,
    ///*Text Billable*/ false),
    ]
    public void TextQualifier_ProperlyQualifiesTexts(long phNum, string contents, string textDateStr, bool callBillable, bool textBillable)
    {
        /*
         *********************************************************************************
         * ASSEMBLE
         *********************************************************************************
         */
        // Assemble timespan objects for use later
        const int fortyFive = 45; // This is used in more than one place
        TimeSpan threeHours = new(3, 0, 0);
        TimeSpan fortyFiveMin = new(0, fortyFive, 0);

        // Assemble the text
        ValueObjects.PhoneNumber phoneNumber = new(phNum);
        DateTimeOffset textDate = DateTimeOffset.Parse(textDateStr);
        Message text = new(phoneNumber, textDate, contents, "This is the source");

        // Assemble the call records
        // Phone number matches
        MessageCallRecord phMatches_AfterTxt_Billable = new(new(phoneNumber), textDate + fortyFiveMin, callBillable);
        MessageCallRecord phMatches_AfterTxt_NonBillable = new(new(phoneNumber), textDate + fortyFiveMin, !callBillable);
        MessageCallRecord phMatches_BeforeTxt_Billable = new(new(phoneNumber), textDate - fortyFiveMin, callBillable);
        MessageCallRecord phMatches_BeforeTxt_NonBillable = new(new(phoneNumber), textDate - fortyFiveMin, !callBillable);

        // No Matches
        MessageCallRecord noMatches_AfterTxt_Billable = new(new(phoneNumber.Number + fortyFive), textDate + fortyFiveMin, callBillable);
        MessageCallRecord noMatches_AfterTxt_NonBillable = new(new(phoneNumber.Number + fortyFive), textDate + fortyFiveMin, !callBillable);
        MessageCallRecord noMatches_BeforeTxt_Billable = new(new(phoneNumber.Number + fortyFive), textDate - fortyFiveMin, callBillable);
        MessageCallRecord noMatches_BeforeTxt_NonBillable = new(new(phoneNumber.Number + fortyFive), textDate - fortyFiveMin, !callBillable);

        // Call Lists
        List<MessageCallRecord> phMatches_BillableAfter =
            [
                phMatches_AfterTxt_Billable,
                phMatches_BeforeTxt_NonBillable,
                noMatches_AfterTxt_Billable, noMatches_AfterTxt_NonBillable, noMatches_BeforeTxt_Billable, noMatches_BeforeTxt_NonBillable
            ];
        List<MessageCallRecord> phMatches_BillableBefore =
            [
                phMatches_BeforeTxt_Billable,
                phMatches_AfterTxt_NonBillable,
                noMatches_AfterTxt_Billable, noMatches_AfterTxt_NonBillable, noMatches_BeforeTxt_Billable, noMatches_BeforeTxt_NonBillable
            ];
        List<MessageCallRecord> phMatches_BillableBoth =
            [
                phMatches_BeforeTxt_Billable, phMatches_AfterTxt_Billable,
                phMatches_AfterTxt_NonBillable, phMatches_BeforeTxt_NonBillable,
                noMatches_AfterTxt_Billable, noMatches_AfterTxt_NonBillable, noMatches_BeforeTxt_Billable, noMatches_BeforeTxt_NonBillable
            ];
        List<MessageCallRecord> noMatches = [noMatches_AfterTxt_Billable, noMatches_AfterTxt_NonBillable, noMatches_BeforeTxt_Billable, noMatches_BeforeTxt_NonBillable];

        // Assemble the customer records
        // The seller doesn't actually matter
        const string sellers = "This string lists the sellers";
        int custNum = 0;
        int subNum = 10;
        const double contractValue = 1.0;
        const bool truer = true;
        DateTimeOffset cxlDates = DateTimeOffset.MaxValue;

        // Create a null customer in the same way as the text qualifier will
        var defaultCustomer = MessageQualifier.NullCustomer;

        // Customers with matching phone number
        // Note that there is no situation in which the customer record occurs before the subscription record. Thus, there is no cAfter_sBefore
        var custNumAfterAfter = ++custNum;
        var subNumAfterAfter = ++subNum;
        CustomerSubscription phMatches_cAfter_sAfter = new(custNumAfterAfter, subNumAfterAfter,
            textDate + threeHours,
            textDate + threeHours + fortyFiveMin,
            phoneNumber, new(phoneNumber.Number + fortyFive), cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);
        var custNumBeforeAfter = ++custNum;
        var subNumBeforeAfter = ++subNum;
        CustomerSubscription phMatches_cBefore_sAfter = new(custNumBeforeAfter, subNumBeforeAfter,
            textDate - threeHours,
            textDate + threeHours - fortyFiveMin,
            new(phoneNumber.Number + fortyFive), phoneNumber, cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);
        var custNumAfterBefore = ++custNum;
        var subNumAfterBefore = ++subNum;
        CustomerSubscription phMatches_cAfter_sBefore = new(custNumAfterBefore, subNumAfterBefore,
            textDate + threeHours + fortyFiveMin + fortyFiveMin,
            textDate - threeHours - threeHours,
            new(phoneNumber.Number + fortyFive), phoneNumber, cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);
        var custNumBeforeBefore = ++custNum;
        var subNumBeforeBefore = ++subNum;
        CustomerSubscription phMatches_cBefore_sBefore = new(custNumBeforeBefore, subNumBeforeBefore,
            textDate - threeHours - fortyFiveMin,
            textDate - threeHours,
            phoneNumber, new(phoneNumber.Number + fortyFive), cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);

        // Customers without matching phone number
        CustomerSubscription noMatches_cAfter_sAfter =
            new(++custNum, ++subNum, textDate + threeHours, textDate + threeHours + fortyFiveMin, new(phoneNumber.Number - fortyFive), new(phoneNumber.Number + fortyFive), cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);
        CustomerSubscription noMatches_cBefore_sAfter =
            new(++custNum, ++subNum, textDate + threeHours, textDate + threeHours + fortyFiveMin, new(phoneNumber.Number - fortyFive), new(phoneNumber.Number + fortyFive), cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);
        CustomerSubscription noMatches_cAfter_sBefore =
            new(++custNum, ++subNum, textDate + threeHours + fortyFiveMin + fortyFiveMin, textDate - threeHours - threeHours, new(phoneNumber.Number + fortyFive), phoneNumber, cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);
        CustomerSubscription noMatches_cBefore_sBefore =
            new(++custNum, ++subNum, textDate + threeHours, textDate + threeHours + fortyFiveMin, new(phoneNumber.Number - fortyFive), new(phoneNumber.Number + fortyFive), cxlDates, cxlDates, truer, truer, truer, contractValue, sellers);

        // Customer lists
        List<CustomerSubscription> phMatches_cAfter_sAfter_List =
            [
                phMatches_cAfter_sAfter,
                noMatches_cAfter_sAfter, noMatches_cBefore_sBefore, noMatches_cBefore_sAfter
            ];
        List<CustomerSubscription> phMatches_cBefore_sAfter_List =
            [
                phMatches_cBefore_sAfter,
                noMatches_cAfter_sAfter, noMatches_cBefore_sBefore, noMatches_cBefore_sAfter
            ];
        List<CustomerSubscription> phMatches_cAfter_sBefore_List =
            [
                phMatches_cAfter_sBefore,
                noMatches_cAfter_sAfter, noMatches_cBefore_sBefore, noMatches_cBefore_sAfter
            ];
        List<CustomerSubscription> phMatches_cBefore_sBefore_List =
            [
                phMatches_cBefore_sBefore,
                noMatches_cAfter_sAfter, noMatches_cBefore_sBefore, noMatches_cBefore_sAfter
            ];
        List<CustomerSubscription> phMatches_All_List =
            [
                phMatches_cBefore_sBefore, phMatches_cBefore_sAfter, phMatches_cAfter_sBefore, phMatches_cAfter_sAfter,
                noMatches_cAfter_sAfter, noMatches_cBefore_sBefore, noMatches_cBefore_sAfter
            ];
        List<CustomerSubscription> noCustomerMatches = [noMatches_cAfter_sAfter, noMatches_cBefore_sAfter, noMatches_cBefore_sBefore];

        /*
         *********************************************************************************
         * ACT
         *********************************************************************************
         */
        // Create all combinations of list types and insert into the qualifier
        // Create text list
        List<IMessage> textList = [text];
        MessageQualifier._test = true;

        // Case 1 V1
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        List<ICallRecord> case1v1_CallList = [.. phMatches_BillableAfter];
        List<ICustomerSubscription> case1v1_List = [.. phMatches_cAfter_sAfter_List];
        List<QualifiedMessageRecord> case1v1 = MessageQualifier.Qualify(textList, case1v1_CallList, case1v1_List);
        var onev1C = textBillable;
        var onev1B = case1v1[0].Billable;
        var onev1L = case1v1[0].IsSalesLead;
        var onev1Id = case1v1[0].Customer.SubscriptionId;
        var onev1SId = subNumAfterAfter;
        // Case 1 V2
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        List<ICallRecord> case1v2_CallList = [.. phMatches_BillableAfter];
        List<ICustomerSubscription> case1v2_List = [.. phMatches_cBefore_sAfter_List];
        List<QualifiedMessageRecord> case1v2 = MessageQualifier.Qualify(textList, case1v2_CallList, case1v2_List);
        var onev2C = textBillable;
        var onev2B = case1v2[0].Billable;
        var onev2L = case1v2[0].IsSalesLead;
        var onev2Id = case1v2[0].Customer.SubscriptionId;
        var onev2SId = subNumBeforeAfter;
        // Case 1 V3
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        List<ICallRecord> case1v3_CallList = [.. phMatches_BillableAfter];
        List<ICustomerSubscription> case1v3_List = [.. phMatches_cBefore_sBefore_List];
        List<QualifiedMessageRecord> case1v3 = MessageQualifier.Qualify(textList, case1v3_CallList, case1v3_List);
        var onev3C = textBillable;
        var onev3B = case1v3[0].Billable;
        var onev3L = case1v3[0].IsSalesLead;
        var onev3Id = case1v3[0].Customer.SubscriptionId;
        var onev3SId = subNumBeforeBefore;
        // Case 1 V4
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        List<ICallRecord> case1v4_CallList = [.. phMatches_BillableAfter];
        List<ICustomerSubscription> case1v4_List = [.. phMatches_All_List];
        List<QualifiedMessageRecord> case1v4 = MessageQualifier.Qualify(textList, case1v4_CallList, case1v4_List);
        var onev4C = textBillable;
        var onev4B = case1v4[0].Billable;
        var onev4L = case1v4[0].IsSalesLead;
        var onev4Id = case1v4[0].Customer.SubscriptionId;
        var onev4SId = subNumBeforeAfter;
        // Case 1 V5
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | no matching phone numbers
        List<ICallRecord> case1v5_CallList = [.. phMatches_BillableAfter];
        List<ICustomerSubscription> case1v5_List = [.. noCustomerMatches];
        List<QualifiedMessageRecord> case1v5 = MessageQualifier.Qualify(textList, case1v5_CallList, case1v5_List);
        var onev5C = textBillable;
        var onev5B = case1v5[0].Billable;
        var onev5L = case1v5[0].IsSalesLead;
        var onev5Id = case1v5[0].Customer.SubscriptionId;
        var onev5SId = defaultCustomer.SubscriptionId;
        // Case 1 V6
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        List<ICallRecord> case1v6_CallList = [.. phMatches_BillableAfter];
        List<ICustomerSubscription> case1v6_List = [.. phMatches_cAfter_sBefore_List];
        List<QualifiedMessageRecord> case1v6 = MessageQualifier.Qualify(textList, case1v6_CallList, case1v6_List);
        var onev6C = textBillable;
        var onev6B = case1v6[0].Billable;
        var onev6L = case1v6[0].IsSalesLead;
        var onev6Id = case1v6[0].Customer.SubscriptionId;
        var onev6SId = subNumAfterBefore;

        // Case 2 V1
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        List<ICallRecord> case2v1_CallList = [.. phMatches_BillableBefore];
        List<ICustomerSubscription> case2v1_List = [.. phMatches_cAfter_sAfter_List];
        List<QualifiedMessageRecord> case2v1 = MessageQualifier.Qualify(textList, case2v1_CallList, case2v1_List);
        var twov1 = textBillable;
        var twov1B = case2v1[0].Billable;
        var twov1L = case2v1[0].IsSalesLead;
        var twov1Id = case2v1[0].Customer.SubscriptionId;
        var twov1SId = subNumAfterAfter;
        // Case 2 V2
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        List<ICallRecord> case2v2_CallList = [.. phMatches_BillableBefore];
        List<ICustomerSubscription> case2v2_List = [.. phMatches_cBefore_sAfter_List];
        List<QualifiedMessageRecord> case2v2 = MessageQualifier.Qualify(textList, case2v2_CallList, case2v2_List);
        var twov2 = textBillable;
        var twov2B = case2v2[0].Billable;
        var twov2L = case2v2[0].IsSalesLead;
        var twov2Id = case2v2[0].Customer.SubscriptionId;
        var twov2SId = subNumBeforeAfter;
        // Case 2 V3
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        List<ICallRecord> case2v3_CallList = [.. phMatches_BillableBefore];
        List<ICustomerSubscription> case2v3_List = [.. phMatches_cBefore_sBefore_List];
        List<QualifiedMessageRecord> case2v3 = MessageQualifier.Qualify(textList, case2v3_CallList, case2v3_List);
        var twov3 = textBillable;
        var twov3B = case2v3[0].Billable;
        var twov3L = case2v3[0].IsSalesLead;
        var twov3Id = case2v3[0].Customer.SubscriptionId;
        var twov3SId = subNumBeforeBefore;
        // Case 2 V4
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        List<ICallRecord> case2v4_CallList = [.. phMatches_BillableBefore];
        List<ICustomerSubscription> case2v4_List = [.. phMatches_All_List];
        List<QualifiedMessageRecord> case2v4 = MessageQualifier.Qualify(textList, case2v4_CallList, case2v4_List);
        var twov4 = textBillable;
        var twov4B = case2v4[0].Billable;
        var twov4L = case2v4[0].IsSalesLead;
        var twov4Id = case2v4[0].Customer.SubscriptionId;
        var twov4SId = subNumBeforeAfter;
        // Case 2 V5
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | no matching phone numbers
        List<ICallRecord> case2v5_CallList = [.. phMatches_BillableBefore];
        List<ICustomerSubscription> case2v5_List = [.. noCustomerMatches];
        List<QualifiedMessageRecord> case2v5 = MessageQualifier.Qualify(textList, case2v5_CallList, case2v5_List);
        var twov5 = textBillable;
        var twov5B = case2v5[0].Billable;
        var twov5L = case2v5[0].IsSalesLead;
        var twov5Id = case2v5[0].Customer.SubscriptionId;
        var twov5SId = defaultCustomer.SubscriptionId;
        // Case 2 V6
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        List<ICallRecord> case2v6_CallList = [.. phMatches_BillableBefore];
        List<ICustomerSubscription> case2v6_List = [.. phMatches_cAfter_sBefore_List];
        List<QualifiedMessageRecord> case2v6 = MessageQualifier.Qualify(textList, case2v6_CallList, case2v6_List);
        var twov6 = textBillable;
        var twov6B = case2v6[0].Billable;
        var twov6L = case2v6[0].IsSalesLead;
        var twov6Id = case2v6[0].Customer.SubscriptionId;
        var twov6SId = subNumAfterBefore;

        // Case 3 V1
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        List<ICallRecord> case3v1_CallList = [.. phMatches_BillableBoth];
        List<ICustomerSubscription> case3v1_List = [.. phMatches_cAfter_sAfter_List];
        List<QualifiedMessageRecord> case3v1 = MessageQualifier.Qualify(textList, case3v1_CallList, case3v1_List);
        var threev1 = textBillable;
        var threev1B = case3v1[0].Billable;
        var threev1L = case3v1[0].IsSalesLead;
        var threev1Id = case3v1[0].Customer.SubscriptionId;
        var threev1SId = subNumAfterAfter;
        // Case 3 V2
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        List<ICallRecord> case3v2_CallList = [.. phMatches_BillableBoth];
        List<ICustomerSubscription> case3v2_List = [.. phMatches_cBefore_sAfter_List];
        List<QualifiedMessageRecord> case3v2 = MessageQualifier.Qualify(textList, case3v2_CallList, case3v2_List);
        var threev2 = textBillable;
        var threev2B = case3v2[0].Billable;
        var threev2L = case3v2[0].IsSalesLead;
        var threev2Id = case3v2[0].Customer.SubscriptionId;
        var threev2SId = subNumBeforeAfter;
        // Case 3 V3
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        List<ICallRecord> case3v3_CallList = [.. phMatches_BillableBoth];
        List<ICustomerSubscription> case3v3_List = [.. phMatches_cBefore_sBefore_List];
        List<QualifiedMessageRecord> case3v3 = MessageQualifier.Qualify(textList, case3v3_CallList, case3v3_List);
        var threev3 = textBillable;
        var threev3B = case3v3[0].Billable;
        var threev3L = case3v3[0].IsSalesLead;
        var threev3Id = case3v3[0].Customer.SubscriptionId;
        var threev3SId = subNumBeforeBefore;
        // Case 3 V4
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        List<ICallRecord> case3v4_CallList = [.. phMatches_BillableBoth];
        List<ICustomerSubscription> case3v4_List = [.. phMatches_All_List];
        List<QualifiedMessageRecord> case3v4 = MessageQualifier.Qualify(textList, case3v4_CallList, case3v4_List);
        var threev4 = textBillable;
        var threev4B = case3v4[0].Billable;
        var threev4L = case3v4[0].IsSalesLead;
        var threev4Id = case3v4[0].Customer.SubscriptionId;
        var threev4SId = subNumBeforeAfter;
        // Case 3 V5
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | no matching phone numbers
        List<ICallRecord> case3v5_CallList = [.. phMatches_BillableBoth];
        List<ICustomerSubscription> case3v5_List = [.. noCustomerMatches];
        List<QualifiedMessageRecord> case3v5 = MessageQualifier.Qualify(textList, case3v5_CallList, case3v5_List);
        var threev5 = textBillable;
        var threev5B = case3v5[0].Billable;
        var threev5L = case3v5[0].IsSalesLead;
        var threev5Id = case3v5[0].Customer.SubscriptionId;
        var threev5SId = defaultCustomer.SubscriptionId;
        // Case 3 V6
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        List<ICallRecord> case3v6_CallList = [.. phMatches_BillableBoth];
        List<ICustomerSubscription> case3v6_List = [.. phMatches_cAfter_sBefore_List];
        List<QualifiedMessageRecord> case3v6 = MessageQualifier.Qualify(textList, case3v6_CallList, case3v6_List);
        var threev6 = textBillable;
        var threev6B = case3v6[0].Billable;
        var threev6L = case3v6[0].IsSalesLead;
        var threev6Id = case3v6[0].Customer.SubscriptionId;
        var threev6SId = subNumAfterBefore;

        // Case 4 V1
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        List<ICallRecord> case4v1_CallList = [.. noMatches];
        List<ICustomerSubscription> case4v1_List = [.. phMatches_cAfter_sAfter_List];
        List<QualifiedMessageRecord> case4v1 = MessageQualifier.Qualify(textList, case4v1_CallList, case4v1_List);
        var fourv1 = textBillable;
        var fourv1B = case4v1[0].Billable;
        var fourv1L = case4v1[0].IsSalesLead;
        var fourv1Id = case4v1[0].Customer.SubscriptionId;
        var fourv1SId = subNumAfterAfter;
        // Case 4 V2
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        List<ICallRecord> case4v2_CallList = [.. noMatches];
        List<ICustomerSubscription> case4v2_List = [.. phMatches_cBefore_sAfter_List];
        List<QualifiedMessageRecord> case4v2 = MessageQualifier.Qualify(textList, case4v2_CallList, case4v2_List);
        var fourv2 = textBillable;
        var fourv2B = case4v2[0].Billable;
        var fourv2L = case4v2[0].IsSalesLead;
        var fourv2Id = case4v2[0].Customer.SubscriptionId;
        var fourv2SId = subNumBeforeAfter;
        // Case 4 V3
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        List<ICallRecord> case4v3_CallList = [.. noMatches];
        List<ICustomerSubscription> case4v3_List = [.. phMatches_cBefore_sBefore_List];
        List<QualifiedMessageRecord> case4v3 = MessageQualifier.Qualify(textList, case4v3_CallList, case4v3_List);
        var fourv3 = textBillable;
        var fourv3B = case4v3[0].Billable;
        var fourv3L = case4v3[0].IsSalesLead;
        var fourv3Id = case4v3[0].Customer.SubscriptionId;
        var fourv3SId = subNumBeforeBefore;
        // Case 4 V4
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        List<ICallRecord> case4v4_CallList = [.. noMatches];
        List<ICustomerSubscription> case4v4_List = [.. phMatches_All_List];
        List<QualifiedMessageRecord> case4v4 = MessageQualifier.Qualify(textList, case4v4_CallList, case4v4_List);
        var fourv4 = textBillable;
        var fourv4B = case4v4[0].Billable;
        var fourv4L = case4v4[0].IsSalesLead;
        var fourv4Id = case4v4[0].Customer.SubscriptionId;
        var fourv4SId = subNumBeforeAfter;
        // Case 4 V5
        // The call list contains | no phone number matches
        // The customer list contains | no matching phone numbers
        List<ICallRecord> case4v5_CallList = [.. noMatches];
        List<ICustomerSubscription> case4v5_List = [.. noCustomerMatches];
        List<QualifiedMessageRecord> case4v5 = MessageQualifier.Qualify(textList, case4v5_CallList, case4v5_List);
        var fourv5 = textBillable;
        var fourv5B = case4v5[0].Billable;
        var fourv5L = case4v5[0].IsSalesLead;
        var fourv5Id = case4v5[0].Customer.SubscriptionId;
        var fourv5SId = defaultCustomer.SubscriptionId;
        // Case 4 V6
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        List<ICallRecord> case4v6_CallList = [.. noMatches];
        List<ICustomerSubscription> case4v6_List = [.. phMatches_cAfter_sBefore_List];
        List<QualifiedMessageRecord> case4v6 = MessageQualifier.Qualify(textList, case4v6_CallList, case4v6_List);
        var fourv6 = textBillable;
        var fourv6B = case4v6[0].Billable;
        var fourv6L = case4v6[0].IsSalesLead;
        var fourv6Id = case4v6[0].Customer.SubscriptionId;
        var fourv6SId = subNumAfterBefore;

        // Place the results in a list, as necessary
        List<List<QualifiedMessageRecord>> results = [case1v1, case1v2, case1v3, case1v4, case1v5, case2v1, case2v2, case2v3, case2v4, case2v5, case3v1, case3v2, case3v3, case3v4, case3v5, case4v1, case4v2, case4v3, case4v4, case4v5];

        /*
         *********************************************************************************
         * ASSERT
         *********************************************************************************
         */
        // We would expect the count of all results to be the same as the count of the text list (in this case, there is only one text)
        results.ForEach(r => Assert.Equal(r.Count, textList.Count));

        // Case 1 V1
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        Assert.Equal(case1v1[0].Billable, textBillable);
        Assert.Equal(case1v1[0].IsSalesLead, textBillable);
        Assert.Equal(case1v1[0].Customer.SubscriptionId, phMatches_cAfter_sAfter.SubscriptionId);
        // Case 1 V2
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        Assert.False(case1v2[0].Billable);
        Assert.False(case1v2[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case1v2[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 1 V3
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        Assert.False(case1v3[0].Billable);
        Assert.Equal(case1v3[0].IsSalesLead, textBillable);
        Assert.Equal(case1v3[0].Customer.SubscriptionId, phMatches_cBefore_sBefore.SubscriptionId);
        // Case 1 V4
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        Assert.False(case1v4[0].Billable);
        Assert.False(case1v4[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case1v4[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 1 V5
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | no matching phone numbers
        Assert.Equal(case1v5[0].Billable, textBillable);
        Assert.Equal(case1v5[0].IsSalesLead, textBillable);
        Assert.Equal(case1v5[0].Customer.SubscriptionId, defaultCustomer.SubscriptionId);
        // Case 1 V6
        // The call list contains | phone number match | billable call | after text
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        Assert.False(case1v6[0].Billable);
        Assert.False(case1v6[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case1v6[0].Customer.SubscriptionId, phMatches_cAfter_sBefore.SubscriptionId);

        // Case 2 V1
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        Assert.False(case2v1[0].Billable);
        Assert.Equal(case2v1[0].IsSalesLead, textBillable);
        Assert.Equal(case2v1[0].Customer.SubscriptionId, phMatches_cAfter_sAfter.SubscriptionId);
        // Case 2 V2
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        Assert.False(case2v2[0].Billable);
        Assert.Equal(case2v2[0].IsSalesLead, textBillable);
        Assert.Equal(case2v2[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 2 V3
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        Assert.False(case2v3[0].Billable);
        Assert.Equal(case2v3[0].IsSalesLead, textBillable);
        Assert.Equal(case2v3[0].Customer.SubscriptionId, phMatches_cBefore_sBefore.SubscriptionId);
        // Case 2 V4
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        Assert.False(case2v4[0].Billable);
        Assert.Equal(case2v4[0].IsSalesLead, textBillable);
        Assert.Equal(case2v4[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 2 V5
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | no matching phone numbers
        Assert.False(case2v5[0].Billable);
        Assert.Equal(case2v5[0].IsSalesLead, textBillable);
        Assert.Equal(case2v5[0].Customer.SubscriptionId, defaultCustomer.SubscriptionId);
        // Case 2 V6
        // The call list contains | phone number match | billable call | before text
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        Assert.False(case2v6[0].Billable);
        Assert.Equal(case2v6[0].IsSalesLead, textBillable);
        Assert.Equal(case2v6[0].Customer.SubscriptionId, phMatches_cAfter_sBefore.SubscriptionId);

        // Case 3 V1
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        Assert.False(case3v1[0].Billable);
        Assert.False(case3v1[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case3v1[0].Customer.SubscriptionId, phMatches_cAfter_sAfter.SubscriptionId);
        // Case 3 V2
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        Assert.False(case3v2[0].Billable);
        Assert.False(case3v2[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case3v2[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 3 V3
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        Assert.False(case3v3[0].Billable);
        Assert.Equal(case3v3[0].IsSalesLead, textBillable);
        Assert.Equal(case3v3[0].Customer.SubscriptionId, phMatches_cBefore_sBefore.SubscriptionId);
        // Case 3 V4
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        Assert.False(case3v4[0].Billable);
        Assert.False(case3v4[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case3v4[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 3 V5
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | no matching phone numbers
        Assert.False(case3v5[0].Billable);
        Assert.Equal(case3v5[0].IsSalesLead, textBillable);
        Assert.Equal(case3v5[0].Customer.SubscriptionId, defaultCustomer.SubscriptionId);
        // Case 3 V6
        // The call list contains | phone number match | billable call | before and after text
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        Assert.False(case3v6[0].Billable);
        Assert.False(case3v6[0].IsSalesLead); // This is false because the billable call after the text occurs before the customer date or subscription
        Assert.Equal(case3v6[0].Customer.SubscriptionId, phMatches_cAfter_sBefore.SubscriptionId);

        // Case 4 V1
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is after the text | subscription is after the text
        Assert.Equal(case4v1[0].Billable, textBillable);
        Assert.Equal(case4v1[0].IsSalesLead, textBillable);
        Assert.Equal(case4v1[0].Customer.SubscriptionId, phMatches_cAfter_sAfter.SubscriptionId);
        // Case 4 V2
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is before the text | subscription is after the text
        Assert.False(case4v2[0].Billable);
        Assert.Equal(case4v2[0].IsSalesLead, textBillable);
        Assert.Equal(case4v2[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 4 V3
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is before the text | subscription is before the text
        Assert.False(case4v3[0].Billable);
        Assert.Equal(case4v3[0].IsSalesLead, textBillable);
        Assert.Equal(case4v3[0].Customer.SubscriptionId, phMatches_cBefore_sBefore.SubscriptionId);
        // Case 4 V4
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | one with date before, two with date after | two with subscription before, one with subscription after
        Assert.False(case4v4[0].Billable);
        Assert.Equal(case4v4[0].IsSalesLead, textBillable);
        Assert.Equal(case4v4[0].Customer.SubscriptionId, phMatches_cBefore_sAfter.SubscriptionId);
        // Case 4 V5
        // The call list contains | no phone number matches
        // The customer list contains | no matching phone numbers
        Assert.Equal(case4v5[0].Billable, textBillable);
        Assert.Equal(case4v5[0].IsSalesLead, textBillable);
        Assert.Equal(case4v5[0].Customer.SubscriptionId, defaultCustomer.SubscriptionId);
        // Case 4 V6
        // The call list contains | no phone number matches
        // The customer list contains | matching phone number | date is after the text | subscription is before the text
        Assert.False(case4v6[0].Billable);
        Assert.Equal(case4v6[0].IsSalesLead, textBillable);
        Assert.Equal(case4v6[0].Customer.SubscriptionId, phMatches_cAfter_sBefore.SubscriptionId);

        // Reset text qualifier test attribute
        MessageQualifier._test = false;
    }
}