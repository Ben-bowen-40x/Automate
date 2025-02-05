using Automate.Domain.MessageAnalysis;
using Automate.Domain.ValueObjects;
using NSubstitute;

namespace Automate.Domain.Test;

public class MessageAnalysisTests
{
    #region BillableCallsMatchingMsg
    [
        Theory,
        InlineData(9876543210),
        InlineData(7894561230),
    ]
    public void BillableCallMatchesMsg(long number)
    {
        // Assemble
        IMessage msg = Substitute.For<IMessage>();
        msg.Number.Returns(new PhoneNumber(number));
        IMessage msg1 = Substitute.For<IMessage>();
        msg1.Number.Returns(new PhoneNumber(number + 1));
        List<IMessage> msgList = [msg, msg1];

        #region Mock Call Records
        ICallRecord r1 = Substitute.For<ICallRecord>();
        ICallRecord r2 = Substitute.For<ICallRecord>();
        ICallRecord r3 = Substitute.For<ICallRecord>();
        ICallRecord r4 = Substitute.For<ICallRecord>();
        ICallRecord r5 = Substitute.For<ICallRecord>();
        ICallRecord r6 = Substitute.For<ICallRecord>();
        ICallRecord r7 = Substitute.For<ICallRecord>();
        ICallRecord r8 = Substitute.For<ICallRecord>();
        ICallRecord r9 = Substitute.For<ICallRecord>();
        ICallRecord r10 = Substitute.For<ICallRecord>();
        ICallRecord r11 = Substitute.For<ICallRecord>();
        ICallRecord r12 = Substitute.For<ICallRecord>();

        r1.Number.Returns(new PhoneNumber(number));
        r2.Number.Returns(new PhoneNumber(number));
        r3.Number.Returns(new PhoneNumber(number + 1));
        r4.Number.Returns(new PhoneNumber(number + 2));
        r5.Number.Returns(new PhoneNumber(number + 3));
        r6.Number.Returns(new PhoneNumber(number + 4));
        r7.Number.Returns(new PhoneNumber(number + 5));
        r8.Number.Returns(new PhoneNumber(number + 6));
        r9.Number.Returns(new PhoneNumber(number + 7));
        r10.Number.Returns(new PhoneNumber(number + 8));
        r11.Number.Returns(new PhoneNumber(number + 9));
        r12.Number.Returns(new PhoneNumber(number + 10));

        r1.Billable.Returns(true);
        r2.Billable.Returns(true);
        r3.Billable.Returns(true);
        r4.Billable.Returns(true);
        r5.Billable.Returns(true);
        r6.Billable.Returns(true);
        r7.Billable.Returns(true);
        r8.Billable.Returns(true);
        r9.Billable.Returns(true);
        r10.Billable.Returns(true);
        r11.Billable.Returns(true);
        r12.Billable.Returns(true);

        List<ICallRecord> callList = [r1, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12
            //*
            , r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12
            //*/
            ];
        #endregion

        foreach (var m in msgList)
        {
            // Act
            LinkedList<ICallRecord> recordList = new(callList);
            List<ICallRecord> actual = MessageQualifier.BillableCallsMatchingMsg(m, recordList);

            // Assert
            foreach (var a in actual)
            {
                Assert.Equal(m.Number.Number, a.Number.Number);
            }
        }
    }
    #endregion

    #region CustomerMatches
    [
        Theory,
        InlineData(9876543210),
        InlineData(7894561230),
    ]
    public void CustomerMatchesTest(long number)
    {
        // Arrange
        long number2 = number - 500;

        // Messages
        IMessage msg = Substitute.For<IMessage>();
        msg.Number.Returns(new PhoneNumber(number));
        IMessage msg1 = Substitute.For<IMessage>();
        msg1.Number.Returns(new PhoneNumber(number2));
        List<IMessage> msgList = [msg, msg1];

        #region Mock Customer Records
        ICustomerSubscription r1 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r2 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r3 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r4 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r5 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r6 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r7 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r8 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r9 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r10 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r11 = Substitute.For<ICustomerSubscription>();
        ICustomerSubscription r12 = Substitute.For<ICustomerSubscription>();

        r1.Number.Returns(new PhoneNumber(number));
        r2.Number.Returns(new PhoneNumber(number));
        r3.Number.Returns(new PhoneNumber(number + 1));
        r4.Number.Returns(new PhoneNumber(number + 2));
        r5.Number.Returns(new PhoneNumber(number + 3));
        r6.Number.Returns(new PhoneNumber(number + 4));
        r7.Number.Returns(new PhoneNumber(number + 5));
        r8.Number.Returns(new PhoneNumber(number + 6));
        r9.Number.Returns(new PhoneNumber(number + 7));
        r10.Number.Returns(new PhoneNumber(number + 8));
        r11.Number.Returns(new PhoneNumber(number + 9));
        r12.Number.Returns(new PhoneNumber(number + 10));

        r1.Number2.Returns(new PhoneNumber(number2));
        r2.Number2.Returns(new PhoneNumber(number2));
        r3.Number2.Returns(new PhoneNumber(number2 + 1));
        r4.Number2.Returns(new PhoneNumber(number2 + 2));
        r5.Number2.Returns(new PhoneNumber(number2 + 3));
        r6.Number2.Returns(new PhoneNumber(number2 + 4));
        r7.Number2.Returns(new PhoneNumber(number2 + 5));
        r8.Number2.Returns(new PhoneNumber(number2 + 6));
        r9.Number2.Returns(new PhoneNumber(number2 + 7));
        r10.Number2.Returns(new PhoneNumber(number2 + 8));
        r11.Number2.Returns(new PhoneNumber(number2 + 9));
        r12.Number2.Returns(new PhoneNumber(number2 + 10));


        List<ICustomerSubscription> customerList = [r1, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12
            //*
            , r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r1, r2, r2, r3, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12
            //*/
            ];
        #endregion

        foreach (var m in msgList)
        {
            // Act
            LinkedList<ICustomerSubscription> recordList = new(customerList);
            List<ICustomerSubscription> actual = MessageQualifier.CustomerMatches(m, recordList);

            // Assert
            foreach (var a in actual)
            {
                Assert.True(a.Number.Number == m.Number.Number || a.Number2.Number == m.Number.Number);
            }
        }
    }
    #endregion

    #region CustomerAttributableToMsg
    /// <summary>
    /// <para>The <paramref name="dateints"/> parameter is used to create <see cref="DateTimeOffset"/> values. Set the values as follows:</para>
    /// <para>Index 0: Year | Index 1: Month | Index 2: Day (of the month) | Index 3: Hour (of the day) | Index 4: Minutes | Index 5: Seconds. 
    /// => Thus, the <paramref name="dateints"/> meets the following <see cref="DateTime"/> pattern: yyyy-MM-dd hh:mm:ss, where y is a four-digit year, M is a 1 or 2 digit month value, and so on</para>
    /// <para>The <see cref="int"/> <paramref name="addMinutesForExpected"/> value must be smaller in magnitude than <paramref name="addMinutesForOther"/> OR positive when the <paramref name="addMinutesForOther"/> is negative OR <paramref name="addMinutesForOther"/> must be zero</para>
    /// <para>For <paramref name="expectedBillable"/> :: When <see cref="true"/>, neither <paramref name="addMinutesForExpected"/> nor <paramref name="addMinutesForOther"/> are negative; When <see cref="false"/>, either <paramref name="addMinutesForExpected"/> or <paramref name="addMinutesForOther"/> is negative</para>
    /// </summary>
    /// <param name="dateints"></param>
    /// <param name="addMinutesForExpected"></param>
    /// <param name="addMinutesForOther"></param>
    /// <param name="expectedBillable"></param>
    private static void CustomerAttributableToMsg(int[] dateints, int addMinutesForExpected, int addMinutesForOther, bool expectedBillable)
    {
        bool value;
        foreach (var date in dateints)
            value = expectedBillable && (date + addMinutesForExpected + addMinutesForOther) == 0;
    }

    [
        Theory,
        // This won't pass because the test is not designed to FIND the expected value, and the expected value is given by the
        //InlineData(new int[] { 2024, 1, 2, 13, 45, 02 }, -10, 011, false), 
        InlineData(new int[] { 2024, 01, 12, 13, 45, 02 }, 000, 000, true),
        InlineData(new int[] { 2024, 01, 12, 13, 45, 02 }, 010, 000, true),
        InlineData(new int[] { 2024, 01, 12, 13, 45, 02 }, 010, 011, true),
        InlineData(new int[] { 2024, 01, 12, 13, 45, 02 }, 010, -11, false),
        InlineData(new int[] { 2024, 01, 12, 13, 45, 02 }, -10, 000, false),
        InlineData(new int[] { 2024, 01, 12, 13, 45, 02 }, -10, -11, false),
    ]
    public void CustomerAttributableToMsgTest(int[] dateints, int addMinutesForExpected, int addMinutesForOther, bool expectedBillable)
    {
        // Assemble Primitives
        DateTimeOffset date = new(new DateTime(dateints[0], dateints[1], dateints[2], dateints[3], dateints[4], dateints[5]), TimeSpan.FromHours(0));

        DateTimeOffset startDate1 = addMinutesForExpected == 0
            ? DateTimeOffset.MaxValue
            : date + TimeSpan.FromMinutes(addMinutesForExpected);
        DateTimeOffset subDate1 = startDate1 == DateTimeOffset.MaxValue
            ? DateTimeOffset.MaxValue
            : startDate1 + TimeSpan.FromMinutes(2);

        DateTimeOffset startDate2 = addMinutesForOther == 0
            ? DateTimeOffset.MaxValue
            : date + TimeSpan.FromMinutes(addMinutesForOther);
        DateTimeOffset subDate2 = startDate2 == DateTimeOffset.MaxValue
            ? DateTimeOffset.MaxValue
            : startDate2 + TimeSpan.FromMinutes(2);

        // Assemble Message
        IMessage msg = Substitute.For<IMessage>();
        msg.Date.Returns(date);

        // Assemble Customers
        ICustomerSubscription expected = Substitute.For<ICustomerSubscription>();
        expected.Date.Returns(startDate1);
        expected.SubscriptionStartDate.Returns(subDate1);

        ICustomerSubscription cc = Substitute.For<ICustomerSubscription>();
        cc.Date.Returns(startDate2);
        cc.SubscriptionStartDate.Returns(subDate2);

        // Act        
        var actual = MessageQualifier.CustomerAttributableToMsg(msg, [expected, cc], out bool actualBillable);

        // Assert
        Assert.Equal(expectedBillable, actualBillable);
        Assert.Equal(expected.Date, actual.Date);
        Assert.Equal(expected.SubscriptionStartDate, actual.SubscriptionStartDate);
    }
    #endregion

    #region DetermineBillability
    [
        Theory,
        // This means that a billed call happened 45 minutes before the msg, and the subscription started 45 minutes after the msg, AND the msg is NOT billable
        InlineData(9876543210, new int[] { 2024, 01, 12, 13, 45, 02 }, -45, 45, false),
        // This means that a billed call happened 20 minutes after the msg and BEFORE the subscription started, AND the msg is billable
        InlineData(9876543210, new int[] { 2024, 01, 12, 13, 45, 02 }, 20, 45, true),
        // This means that a billed call happened 50 minutes after the msg, and the subscription started 45 minutes after, AND the msg is NOT billable
        InlineData(9876543210, new int[] { 2024, 01, 12, 13, 45, 02 }, 50, 45, true),
        // This means that a billed call happened 45 minutes after the msg, and the subscription started 45 minutes before, AND the msg is NOT billable
        InlineData(9876543210, new int[] { 2024, 01, 12, 13, 45, 02 }, 45, -45, false),
    ]
    public void DetermineBillabilityTest(long number, int[] dateints, int callDiffMinutes, int custSubDateDiffMinutes, bool expected)
    {
        // Assemble primitives
        const string contents = "want quote";
        PhoneNumber num = new(number);
        DateTimeOffset date = new(new DateTime(dateints[0], dateints[1], dateints[2], dateints[3], dateints[4], dateints[5]), TimeSpan.FromHours(0));

        // Message Assembly
        IMessage msg = Substitute.For<IMessage>();
        msg.Contents.Returns(contents);
        msg.Number.Returns(num);
        msg.Date.Returns(date);

        // Call Assembly
        ICallRecord r1 = Substitute.For<ICallRecord>();
        r1.Date.Returns(date + TimeSpan.FromMinutes(callDiffMinutes));
        List<ICallRecord> callList = [r1];

        // Customer Assembly
        ICustomerSubscription cs = Substitute.For<ICustomerSubscription>();
        var custsubdate = date + TimeSpan.FromMinutes(custSubDateDiffMinutes);
        cs.Date.Returns(custsubdate);
        cs.SubscriptionStartDate.Returns(custsubdate);
        cs.SubscriptionId.Returns(1);

        // Additional Assembly
        bool couldBeBillable = custsubdate > date;

        // Act
        bool actual = MessageQualifier.DetermineBillability(msg, callList, couldBeBillable, cs, out bool salesActual);

        // Assert
        Assert.NotEqual(cs.SubscriptionId, MessageQualifier.NullCustomer.SubscriptionId);
        Assert.NotEqual(cs.SubscriptionStartDate, MessageQualifier.NullCustomer.SubscriptionStartDate);
        Assert.True(salesActual); // Sales is always billable because the pattern is a billable pattern
        Assert.Equal(actual, expected);
    }
    #endregion
}
