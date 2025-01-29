using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.QualifiedMessageTranslate;
using Automate.Translation.Test.ValueObjectsTranslationsTests;
using NSubstitute;

namespace Automate.Translation.Test;

public class QualifiedMsgTranslateTest
{
    [
        Theory,
        // long Number, DateTimeOffset Date, string? Contents, string? Source, bool ImLead, bool SalesLead, long CustomeriD, long subId, bool subIsActive, DateTime CustomerStartDate, DateTime CustomerCancelDate, bool CompletedInitial, double ContractValue, DateTime SubStartDate, DateTime SubCancelDate, string? Sellers
        InlineData
        (
            //year, month, day, hour, minutes, seconds
            123456789,                             //long Number
            new int[] { 2014, 01, 15, 21, 35, 21 },     //DateTimeOffset Date 
            null,                                       //string? Contents
            null,                                       //string? Source 
            false,                                      //bool ImLead
            false,                                      //bool SalesLead
            987654321,                                  //long CustomerId
            654987321,                                  //long SubId
            true,                                       //bool SubIsActive
            new int[] { 2014, 02, 15, 21, 35, 21 },     //DateTime CustomerStartDate
            new int[] { 0, 0, 0, 0, 0, 0 },             //DateTime CustomerCancelDate
            true,                                       //CompletedInitial
            123.0,                                      //ContractValue
            new int[] { 2014, 02, 15, 21, 35, 21 },     //DateTime SubStartDate
            new int[] { 0, 0, 0, 0, 0, 0 },             //DateTime SubCancelDate
            null                                        //Sellers
        ),
    ]
    public void IQualifiedMessageTranslate_TranslatesTo_QualifiedMessageRecord(long number, int[] dateInts, string? contents, string? source, bool imLead, bool salesLead, long custId, long subId, bool subIsActive, int[] custDateInts, int[] custCxlDateInts, bool completedInitial, double contractValue, int[] subStartInts, int[] subCxlInts, string? sellers)
    {
        // Convert datetimes
        DateTime date = DTOConvertTests.MakeDateFromIntArray(dateInts[0], dateInts[1], dateInts[2], dateInts[3], dateInts[4], dateInts[5]);
        DateTime custDate = DTOConvertTests.MakeDateFromIntArray(custDateInts[0], custDateInts[1], custDateInts[2], custDateInts[3], custDateInts[4], custDateInts[5]);
        DateTime custCxlDate = DTOConvertTests.MakeDateFromIntArray(custCxlDateInts[0], custCxlDateInts[1], custCxlDateInts[2], custCxlDateInts[3], custCxlDateInts[4], custCxlDateInts[5]);
        DateTime subStartDate = DTOConvertTests.MakeDateFromIntArray(subStartInts[0], subStartInts[1], subStartInts[2], subStartInts[3], subStartInts[4], subStartInts[5]);
        DateTime subCxlDate = DTOConvertTests.MakeDateFromIntArray(subCxlInts[0], subCxlInts[1], subCxlInts[2], subCxlInts[3], subCxlInts[4], subCxlInts[5]);

        // Set up a mock object using given parameters
        IQualifiedMessageTranslate qmock = Substitute.For<IQualifiedMessageTranslate>();
        qmock.Number.Returns(number);
        qmock.Date.Returns(date);
        qmock.Contents.Returns(contents);
        qmock.Source.Returns(source);
        qmock.ImLead.Returns(imLead);
        qmock.SalesLead.Returns(salesLead);
        qmock.CustomerID.Returns(custId);
        qmock.SubId.Returns(subId);
        qmock.SubIsActive.Returns(subIsActive);
        qmock.CustomerStartDate.Returns(custDate);
        qmock.CustomerCancelDate.Returns(custCxlDate);
        qmock.CompletedInitial.Returns(completedInitial);
        qmock.ContractValue.Returns(contractValue);
        qmock.SubStartDate.Returns(subStartDate);
        qmock.SubCancelDate.Returns(subCxlDate);
        qmock.Sellers.Returns(sellers);

        // Set up expected value for IMessage
        IMessage expectedMessage = Substitute.For<IMessage>();
        expectedMessage.Number.Returns(PhoneNumberTranslate.Translate(number));
        expectedMessage.Contents.Returns(contents);
        expectedMessage.Source = source is null ? string.Empty : source;

        // Set up expected value for ICustomerSubscription
        ICustomerSubscription expectedSubscription = Substitute.For<ICustomerSubscription>();
        expectedSubscription.CustomerId.Returns(custId);
        expectedSubscription.SubscriptionId.Returns(subId);
        expectedSubscription.SubscriptionStartDate.Returns(subStartDate); //.Returns();
        expectedSubscription.Number.Returns(expectedMessage.Number);
        expectedSubscription.Number2.Returns(PhoneNumberTranslate.Default);
        expectedSubscription.CustomerCancelDate.Returns(custCxlDate);
        expectedSubscription.SubscriptionCancelDate.Returns(subCxlDate);
        expectedSubscription.CustomerActive.Returns(Arg.Any<bool>());
        expectedSubscription.SubscriptionActive.Returns(subIsActive);
        expectedSubscription.InitialCompleted.Returns(completedInitial);
        expectedSubscription.ContractValue.Returns(contractValue);
        expectedSubscription.Sellers.Returns(sellers);

        // Set up expected value for QualifiedMessageRecord
        var expectedRecord = new QualifiedMessageRecord(expectedMessage, expectedSubscription, imLead, salesLead);

        // Act on the mock
        IMessage actualMessage = qmock.Convert<IMsgDTONumberLong, IMessage>();
        QualifiedMessageRecord actualRecord = qmock.Translate();

        // Confirm proper execution
        actualMessage.Returns(expectedMessage);
        actualRecord.Returns(expectedRecord);
    }
}
