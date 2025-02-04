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
}
