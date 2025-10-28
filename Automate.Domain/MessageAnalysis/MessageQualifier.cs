using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;

namespace Automate.Domain.MessageAnalysis;

public class MessageQualifier
{
    #region Public
    /// <summary>
    /// Qualifies the provided <paramref name="msgs"/> using the <paramref name="callRecords"/> and <paramref name="customerRecords"/>
    /// </summary>
    /// <param name="msgs"></param>
    /// <param name="callRecords"></param>
    /// <param name="customerRecords"></param>
    /// <returns></returns>
    public static List<QualifiedMessageRecord> Qualify(List<IMessage> msgs, List<ICallRecord> callRecords, List<ICustomerSubscription> customerRecords, MessageType type)
    {
        // Prepare logger
        object sender = new MessageQualifier();
        string member = nameof(Qualify);
        string nam = GetFullName.GetMemberName(sender, member);
        string name = _test ? nam + " Test" : nam;
        StringLogger.AddLog($"Started {name}");

        // Iterate through each message and qualify it
        List<QualifiedMessageRecord> result = new(msgs.Count);
        LinkedList<ICallRecord> callList = new(callRecords);
        LinkedList<ICustomerSubscription> customerList = new(customerRecords);
        foreach (IMessage msg in msgs)
        {
            // Match message with calls
            List<ICallRecord> callMsgPhoneMatch = BillableCallsMatchingMsg(msg, callList);

            // Match message with customers to find a single matching customer record
            List<ICustomerSubscription> matches = CustomerMatches(msg, customerList);
            ICustomerSubscription match = CustomerAttributableToMsg(msg, matches, out bool couldBeBillable);

            // Use these lists to determine whether the message is billable and whether the message is a sales lead
            bool billable = DetermineBillability(msg, callMsgPhoneMatch, couldBeBillable, match, out bool isSalesLead);

            // Add to Result
            result.Add(new QualifiedMessageRecord(msg, match, billable && couldBeBillable, isSalesLead, type));
        }

        // Note the end of the log
        StringLogger.AddLog($"Ended {name}", $"Total number of messages in list: {msgs.Count}");

        return result;
    }
    #endregion

    #region Internal
    /// <summary>
    /// This is used exclusively for testing and in the class itself
    /// </summary>
    /// <returns>
    /// <para><see cref="ICustomerSubscription"/> with default values</para>
    /// <para><see cref="ICustomerSubscription.Date"/> == <see cref="DateTimeOffset.MaxValue"/></para> 
    /// <para><see cref="ICustomerSubscription.SubscriptionStartDate"/> == <see cref="DateTimeOffset.MaxValue"/></para>
    /// </returns>
    internal static ICustomerSubscription NullCustomer => _nullCustomer ??= new CustomerSubscription(0, 0, DateTimeOffset.MaxValue, DateTimeOffset.MaxValue, new(0), new(0), DateTimeOffset.MinValue, DateTimeOffset.MinValue, false, false, false, 0.0, "");

    private static ICustomerSubscription? _nullCustomer;

    /// <summary>
    /// <para>Note that this <see cref="internal"/> item should never be changed outside of test situations</para> 
    /// </summary>
    internal static bool _test = false;
    #endregion

    #region Can be tested
    /// <summary>
    /// <para>Finds one instance of type <see cref="ICallRecord"/> <paramref name="callRecords"/> such that it matches <paramref name="message"/></para>
    /// <para>  <see cref="ICallRecord.Billable"/> == <see cref="true"/> and <see cref="ICallRecord.Number"/> == <see cref="IMessage.Number"/></para>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="callRecords"></param>
    /// <returns>
    /// <see cref="List"/> of <see cref="ICallRecord"/>
    /// </returns>
    internal static List<ICallRecord> BillableCallsMatchingMsg(IMessage message, LinkedList<ICallRecord> callRecords)
    {
        // Iterate through the calls to find calls whose phone number matches the message
        List<ICallRecord> result = new(callRecords.Count / 2); // This excessively high capacity ensures we never have an operation of O(n) complexity
        List<ICallRecord> remove = new(callRecords.Count / 2);
        foreach (ICallRecord record in callRecords)
        {
            // If the numbers match and the call is billable, save the index of the callrecord
            if (record.Number.Number == message.Number.Number)
            {
                if (record.Billable)
                    result.Add(record);
                remove.Add(record);
            }
        }

        // In all cases where the numbers match, remove the call from the list of calls through which we're required to iterate
        foreach (var r in remove)
            callRecords.Remove(r);

        return result;
    }

    /// <summary>
    /// <para>This creates a list of customers relevant to a specific test</para>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="customerRecords"></param>
    /// <returns>
    /// <see cref="List"/> of <see cref="ICustomerSubscription"/> that reasonably match the provided <paramref name="message"/>
    /// </returns>
    public static List<ICustomerSubscription> CustomerMatches(IMessage message, LinkedList<ICustomerSubscription> customerRecords)
    {
        // Iterate through the customer list to create a list of customers relevant to this specific message
        // Customer number must match AND customer must have become a customer before the message
        // Iterate backward through the list so that records that have been found can be removed
        List<ICustomerSubscription> matches = new(customerRecords.Count / 2);
        foreach (var record in customerRecords)
        {
            bool numberMatches = record.Number.Number == message.Number.Number || record.Number2.Number == message.Number.Number;
            if (numberMatches) // Add the matching customer to the result
                matches.Add(record);
            }

        // Remove the number of customers that have to be iterated through later
        foreach (var m in matches)
            customerRecords.Remove(m);

        return matches;
    }

    /// <summary>
    /// Uses the <see cref="DateTimeOffset"/> attributes of each of the <paramref name="matches"/> and the <paramref name="message"/> to determine which of the <paramref name="matches"/> can be reasonably attributed to the <paramref name="message"/> based on when each of the <paramref name="matches"/> occurred.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="matches"></param>
    /// <param name="possibleBillable"></param>
    /// <returns> 
    /// <para>Out parameter <paramref name="possibleBillable"/> evaluates to <see cref="false"/> when any <see cref="ICustomerSubscription"/> occurs before the <paramref name="message"/>, otherwise <paramref name="possibleBillable"/> evaluates to <see cref="true"/>.</para>
    /// The returned <see cref="ICustomerSubscription"/> value can be described in the following way:
    /// <para>If there is at least one <see cref="ICustomerSubscription"/> after the <paramref name="message"/>, return the <see cref="ICustomerSubscription"/> that occurred first chronologically after the <paramref name="message"/></para>
    /// <para>If there is no <see cref="ICustomerSubscription"/> after the <paramref name="message"/>, return the most recent <see cref="ICustomerSubscription"/> before the <paramref name="message"/></para>
    /// <para>If there are no customers, return <see cref="NullCustomer"/>, which is an <see cref="ICustomerSubscription"/> with all values set to a specific <see cref="default"/></para>
    /// </returns>
    public static ICustomerSubscription CustomerAttributableToMsg(IMessage message, List<ICustomerSubscription> matches, out bool possibleBillable)
    {
        // We will use a null customer for a default
        ICustomerSubscription nullCustomer = NullCustomer;

        // Create lists that split the matching customer records into categories based on when they occurred in relation to the message
        List<ICustomerSubscription> dAfter_sAfter = [.. matches.Where(c => c.Date != DateTimeOffset.MaxValue & c.Date != DateTimeOffset.MinValue & c.Date > message.Date & c.SubscriptionStartDate > message.Date)];
        List<ICustomerSubscription> dAfter_sBefore = [.. matches.Where(c => c.Date != DateTimeOffset.MaxValue & c.Date != DateTimeOffset.MinValue & c.Date > message.Date & c.SubscriptionStartDate < message.Date)];
        List<ICustomerSubscription> dBefore_sAfter = [.. matches.Where(c => c.Date != DateTimeOffset.MaxValue & c.Date != DateTimeOffset.MinValue & c.Date < message.Date & c.SubscriptionStartDate > message.Date)];
        List<ICustomerSubscription> dBefore_sBefore = [.. matches.Where(c => c.Date != DateTimeOffset.MaxValue & c.Date != DateTimeOffset.MinValue & c.Date < message.Date & c.SubscriptionStartDate < message.Date)];

        // Try to find out whether any of the matches occurred before the message
        bool occurredBefore = dAfter_sBefore.Count > 0 || dBefore_sAfter.Count > 0 || dBefore_sBefore.Count > 0;
        if (occurredBefore)
            possibleBillable = false;
        else
            possibleBillable = true;

        // If there are no customer matches, then return null customer
        if (matches.Count == 0 || (dAfter_sAfter.Count == 0 && dAfter_sBefore.Count == 0 && dBefore_sAfter.Count == 0 && dBefore_sBefore.Count == 0))
            return nullCustomer;

        // At this point, we only care about customers with both dates before the message if there are no customers with at least one date after the message
        ICustomerSubscription result;
        if (dAfter_sAfter.Count > 0 || dAfter_sBefore.Count > 0 || dBefore_sAfter.Count > 0)
            result = GetFirstCustomerAfterMsg(nullCustomer, dAfter_sAfter, dAfter_sBefore, dBefore_sAfter);
        else
            result = GetMostRecentCustomerBeforeMsg(dBefore_sBefore);

        // Return result
        return result;

        // Local functions
        static ICustomerSubscription GetFirstCustomerAfterMsg(ICustomerSubscription nullCustomer, List<ICustomerSubscription> dAfter_sAfter, List<ICustomerSubscription> dAfter_sBefore, List<ICustomerSubscription> dBefore_sAfter)
        {
            // Find the message that occurred after the message first, by whichever date
            ICustomerSubscription afterAfter = nullCustomer;
            foreach (var record in dAfter_sAfter)
            {
                // Booleans of all combinations.
                bool recordDateFirst =
                    record.Date <= afterAfter.Date
                    && record.Date <= afterAfter.SubscriptionStartDate;
                bool recordSubFirst =
                    record.SubscriptionStartDate <= afterAfter.Date
                    && record.SubscriptionStartDate <= afterAfter.SubscriptionStartDate;
                if (recordDateFirst || recordSubFirst)
                    afterAfter = record;
            }

            // Now we must find the customer record with the Date soonest after the message
            ICustomerSubscription afterBefore = nullCustomer;
            foreach (var record in dAfter_sBefore)
            {
                // In these combinations, we can't care about the subscription because it's before the message
                bool recordDateFirst = record.Date < afterBefore.Date;
                bool subDateLatest =
                    record.Date == afterBefore.Date
                    && record.SubscriptionStartDate >= afterBefore.SubscriptionStartDate;
                if (recordDateFirst || subDateLatest)
                    afterBefore = record;
            }

            // Now we must find the record with the subscription date soonest after the message
            ICustomerSubscription beforeAfter = nullCustomer;
            foreach (var record in dBefore_sAfter)
            {
                // In these combinations, we can't use the customer start date because it's before the message
                bool recordSubFirst = record.SubscriptionStartDate < beforeAfter.SubscriptionStartDate;
                bool customerRecordLatest = 
                    record.SubscriptionStartDate == beforeAfter.SubscriptionStartDate 
                    && record.Date >= beforeAfter.Date;
                if (recordSubFirst || customerRecordLatest)
                    beforeAfter = record;
            }

            // Now that we have the most recent record from all three, we need to found out which of these three records happened most recently after the message
            // List the boolean combinations
            bool afterAfterDate_First =
                afterAfter.Date < afterBefore.Date
                && afterAfter.Date < beforeAfter.SubscriptionStartDate;
            bool afterAfterSub_First =
                afterAfter.SubscriptionStartDate < afterBefore.Date
                && afterAfter.SubscriptionStartDate < beforeAfter.SubscriptionStartDate;
            bool afterBeforeDate_First =
                afterBefore.Date < afterAfter.Date
                && afterBefore.Date < afterAfter.SubscriptionStartDate
                && afterBefore.Date < beforeAfter.SubscriptionStartDate;
            bool beforeAfterSub_First =
                beforeAfter.SubscriptionStartDate < afterAfter.Date
                && beforeAfter.SubscriptionStartDate < afterAfter.SubscriptionStartDate
                && beforeAfter.SubscriptionStartDate < afterBefore.Date;

            // Return the first one
            if (afterAfterDate_First || afterAfterSub_First)
                return afterAfter;
            else if (afterBeforeDate_First)
                return afterBefore;
            else if (beforeAfterSub_First)
                return beforeAfter;
            return nullCustomer;
        }

        static ICustomerSubscription GetMostRecentCustomerBeforeMsg(List<ICustomerSubscription> dBefore_sBefore)
        {
            // There are no customer records after the message, but there must be at least one before the message
            // This means we should return the customer account that occurred most closely to the message
            var recent = dBefore_sBefore[0];
            foreach (var record in dBefore_sBefore)
            {
                if (record.Date >= recent.Date || record.SubscriptionStartDate >= recent.SubscriptionStartDate)
                    recent = record;
            }
            return recent;
        }
    }

    internal static bool DetermineBillability(IMessage message, List<ICallRecord> billedCalls, bool couldBeBillable, ICustomerSubscription match, out bool isSalesLead)
    {
        // Assume the message is billable, then prove that it is non billable
        bool billable = true;

        // Determine messager intent
        ClassificationResult patternResult = MessagePatterns.Billable(message.Contents);
        bool billableByPattern = patternResult.Result;

        // If intent was indeterminate, we need to log that
        if (patternResult.NoMatches)
            StringLogger.AddLog("Message contents matched no regular expressions", $"Phone Number: {message.Number}", $"Contents: {message.Contents}");

        // There are no billable calls after the message and before the subscription
        bool notBilledAfterTxtB4Cust = billedCalls.Where(b => AfterMsgBeforeSubConditions(message, match, b)).ToList().Count == 0;

        // The number of billable calls before the message
        int billedBeforeMsg = billedCalls.Where(b => b.Date < message.Date).ToList().Count;

        // If the customer start date occurred before the message, then the message is not billable
        if (match.Date < message.Date || !couldBeBillable || billedBeforeMsg > 0)
            billable = false;

        // If the call is not billable by pattern, but it is billable by all other standards, it is assumed to be non billable
        billable &= billableByPattern;

        // If the message is not billable, there is one other scenario by which it may be a sales lead
        // The message must be billable by pattern and a billable call cannot have occurred after the message and before the subscription
        isSalesLead = billableByPattern && notBilledAfterTxtB4Cust;

        // If the message is billable, then it is automatically a sales lead
        if (billable)
            isSalesLead = true;

        // Return the billability
        return billable;

        // Locals
        static bool AfterMsgBeforeSubConditions(IMessage message, ICustomerSubscription match, ICallRecord call)
        {
            bool callIsAfterTxt = call.Date > message.Date;
            bool messageIsAfterCust =
                match.Date < match.SubscriptionStartDate
                ? message.Date < match.Date
                : message.Date < match.SubscriptionStartDate;
            bool callIsBeforeCust =
                match.Date < match.SubscriptionStartDate
                ? call.Date < match.SubscriptionStartDate
                : call.Date < match.Date;
            bool subIsNotDefault = match.SubscriptionStartDate != NullCustomer.SubscriptionStartDate & match.SubscriptionId != NullCustomer.SubscriptionId;

            if (messageIsAfterCust && callIsAfterTxt && callIsBeforeCust && subIsNotDefault)
                StringLogger.AddLog("Message occurred before the call, and the call occurred before the customer start date or the subscription start date. This can cause an odd reporting situation because both the message and the call are attributable to either the new customer or new subscription. So, which gets the credit, the call, the message, or both?",
                    $"Message: (Message Number) {message.Number} | (Message Date) {message.Date.ToString(DateTimeStrings.InternalDateTimeOffset)}",
                    $"Customer Record: (Customer Start Date) {match.Date.ToString(DateTimeStrings.InternalDateTimeOffset)} | (Customer Subscription Date) {match.SubscriptionStartDate.ToString(DateTimeStrings.InternalDateTimeOffset)}",
                    $"Call Record: (Call Date) {call.Date.ToString(DateTimeStrings.InternalDateTimeOffset)}");

            return callIsAfterTxt && callIsBeforeCust && subIsNotDefault;
        }
    }
    #endregion
}
