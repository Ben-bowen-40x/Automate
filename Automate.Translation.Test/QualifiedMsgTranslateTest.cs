using Automate.Domain.ValueObjects;
using Automate.Translation.CustomerTranslate;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.QualifiedMessageTranslate;
using NSubstitute;

namespace Automate.Translation.Test;

public class QualifiedMsgTranslateTest
{
    #region Theory
    [
        Theory,
        // long Number, DateTimeOffset Date, string? Contents, string? Source, bool ImLead, bool SalesLead, long CustomeriD, long subId, bool subIsActive, DateTime CustomerStartDate, DateTime CustomerCancelDate, bool CompletedInitial, double ContractValue, DateTime SubStartDate, DateTime SubCancelDate, string? Sellers
        InlineData
        (
            //year, month, day, hour, minutes, seconds, offset hours
            9876543210,                            //long Number
            new int[] { 2014, 01, 15, 21, 35, 21, -7 }, //DateTimeOffset Date 
            null,                                       //string? Contents
            null,                                       //string? Source 
            false,                                      //bool ImLead
            false,                                      //bool SalesLead
            987654321,                                  //long CustomerId
            654987321,                                  //long SubId
            true,                                       //bool SubIsActive
            new int[] { 2014, 02, 15, 21, 35, 21, -7 }, //DateTimeOffset CustomerStartDate
            new int[] { 0, 0, 0, 0, 0, 0, 0 },          //DateTime CustomerCancelDate
            true,                                       //CompletedInitial
            123.01,                                     //ContractValue
            new int[] { 2014, 02, 15, 21, 35, 21, -7 }, //DateTimeOffset SubStartDate
            new int[] { 0, 0, 0, 0, 0, 0, 0 },          //DateTime SubCancelDate
            null                                        //Sellers
        ),
        InlineData
        (
            //year, month, day, hour, minutes, seconds
            8876543210,                            //long Number
            new int[] { 2013, 01, 15, 21, 35, 21, -7 }, //DateTimeOffset Date 
            null,                                       //string? Contents
            null,                                       //string? Source 
            false,                                      //bool ImLead
            false,                                      //bool SalesLead
            987654320,                                  //long CustomerId
            654987325,                                  //long SubId
            true,                                       //bool SubIsActive
            new int[] { 2013, 02, 15, 21, 35, 21, -7 }, //DateTimeOffset CustomerStartDate
            new int[] { 0, 0, 0, 0, 0, 0, 0 },           //DateTime CustomerCancelDate
            true,                                       //CompletedInitial
            4581.2234892,                               //ContractValue
            new int[] { 2013, 02, 15, 21, 35, 21, -7 }, //DateTimeOffset SubStartDate
            new int[] { 0, 0, 0, 0, 0, 0, 0 },          //DateTime SubCancelDate
            "Ham, Sam, Jam, Alacazam"                   //Sellers
        ),
    ]
    #endregion
    public void IQualifiedMessageTranslate_TranslatesTo_QualifiedMessageRecord(long number, int[] dateInts, string? contents, string? source, bool imLead, bool salesLead, long custId, long subId, bool subIsActive, int[] custDateInts, int[] custCxlDateInts, bool completedInitial, double contractValue, int[] subStartInts, int[] subCxlInts, string? sellers)
    {
        #region Assemble
        // Convert datetimes
        DateTimeOffset date = IntArrToDTO(dateInts[0], dateInts[1], dateInts[2], dateInts[3], dateInts[4], dateInts[5], dateInts[6]);
        DateTimeOffset custDate = IntArrToDTO(custDateInts[0], custDateInts[1], custDateInts[2], custDateInts[3], custDateInts[4], custDateInts[5], custDateInts[6]);
        DateTimeOffset custCxlDate = IntArrToDTO(custCxlDateInts[0], custCxlDateInts[1], custCxlDateInts[2], custCxlDateInts[3], custCxlDateInts[4], custCxlDateInts[5], custCxlDateInts[6]);
        DateTimeOffset subStartDate = IntArrToDTO(subStartInts[0], subStartInts[1], subStartInts[2], subStartInts[3], subStartInts[4], subStartInts[5], subStartInts[6]);
        DateTimeOffset subCxlDate = IntArrToDTO(subCxlInts[0], subCxlInts[1], subCxlInts[2], subCxlInts[3], subCxlInts[4], subCxlInts[5], subCxlInts[6]);

        // Convert PhoneNumber
        PhoneNumber phNumber = PhoneNumberTranslate.Translate(number);

        // Set up a mock object using given parameters
        IQualifiedMessageTranslate toBeTranslated = Substitute.For<IQualifiedMessageTranslate>();
        toBeTranslated.Number.Returns(number);
        toBeTranslated.Phone1.Returns(number);
        toBeTranslated.Phone2.Returns(number);
        toBeTranslated.Date.Returns(date);
        toBeTranslated.Contents.Returns(contents);
        toBeTranslated.Source.Returns(source);
        toBeTranslated.ImLead.Returns(imLead);
        toBeTranslated.SalesLead.Returns(salesLead);
        toBeTranslated.CustomerID.Returns(custId);
        toBeTranslated.SubId.Returns(subId);
        toBeTranslated.SubIsActive.Returns(subIsActive);
        toBeTranslated.CustomerStartDate.Returns(custDate);
        toBeTranslated.CustomerCancelDate.Returns(custCxlDate);
        toBeTranslated.CompletedInitial.Returns(completedInitial);
        toBeTranslated.ContractValue.Returns(contractValue);
        toBeTranslated.SubStartDate.Returns(subStartDate);
        toBeTranslated.SubCancelDate.Returns(subCxlDate);
        toBeTranslated.Sellers.Returns(sellers);

        // Set up expected value for IMessage
        IMessage expectedMessage = Substitute.For<IMessage>();
        expectedMessage.Number.Returns(phNumber); // Ensures that the object reference is not null
        expectedMessage.Contents.Returns(contents is null ? string.Empty : source);
        expectedMessage.Source.Returns(source is null ? string.Empty : source);

        // Set up expected value for ICustomerSubscription
        ICustomerSubscription expectedSubscription = Substitute.For<ICustomerSubscription>();
        expectedSubscription.CustomerId.Returns(custId);
        expectedSubscription.SubscriptionId.Returns(subId);
        expectedSubscription.SubscriptionStartDate.Returns(subStartDate);
        expectedSubscription.Number.Returns(phNumber);
        expectedSubscription.Number2.Returns(phNumber);
        expectedSubscription.CustomerCancelDate.Returns(custCxlDate);
        expectedSubscription.SubscriptionCancelDate.Returns(subCxlDate);
        expectedSubscription.CustomerActive.Returns(subIsActive);
        expectedSubscription.SubscriptionActive.Returns(subIsActive);
        expectedSubscription.InitialCompleted.Returns(completedInitial);
        expectedSubscription.ContractValue.Returns(contractValue);
        expectedSubscription.Sellers.Returns(CustomerSubscriptionTranslate.VerifySeller(sellers));

        // Set up expected value for QualifiedMessageRecord
        MessageType type = MessageType.Leaf;
        QualifiedMessageRecord expectedRecord = new(expectedMessage, expectedSubscription, imLead, salesLead, type);
        #endregion

        // Act
        QualifiedMessageRecord actualRecord = toBeTranslated.Translate(type);

        #region Assert
        // Confirm proper execution
        // Check that the expected and actual message values are the same
        Assert.Equal(expectedMessage.Number.Number, actualRecord.Message.Number.Number);
        Assert.Equal(expectedMessage.Contents, actualRecord.Message.Contents);
        Assert.Equal(expectedMessage.Source, actualRecord.Message.Source);

        // Check that the expected and actual subscription values are the same
        Assert.Equal(expectedSubscription.CustomerId, actualRecord.Customer.CustomerId);
        Assert.Equal(expectedSubscription.SubscriptionId, actualRecord.Customer.SubscriptionId);
        Assert.Equal(expectedSubscription.SubscriptionStartDate, actualRecord.Customer.SubscriptionStartDate);
        Assert.Equal(expectedSubscription.Number.Number, actualRecord.Customer.Number.Number);
        Assert.Equal(expectedSubscription.Number2.Number, actualRecord.Customer.Number2.Number);
        Assert.Equal(expectedSubscription.CustomerCancelDate, actualRecord.Customer.CustomerCancelDate);
        Assert.Equal(expectedSubscription.SubscriptionCancelDate, actualRecord.Customer.SubscriptionCancelDate);
        Assert.Equal(expectedSubscription.CustomerActive, actualRecord.Customer.CustomerActive);
        Assert.Equal(expectedSubscription.SubscriptionActive, actualRecord.Customer.SubscriptionActive);
        Assert.Equal(expectedSubscription.InitialCompleted, actualRecord.Customer.InitialCompleted);
        Assert.Equal(expectedSubscription.ContractValue, actualRecord.Customer.ContractValue);
        Assert.Equal(expectedSubscription.Sellers, actualRecord.Customer.Sellers);

        // Check that expected and actual QualifiedMessageRecord values are the same (These values are unique to the QualifiedMessageRecord)
        Assert.Equal(expectedRecord.IsSalesLead, actualRecord.IsSalesLead);
        Assert.Equal(expectedRecord.Billable, actualRecord.Billable);
        #endregion
    }
    public static DateTimeOffset IntArrToDTO(int year, int month, int day, int hour, int minutes, int seconds, int offset)
    {
        return year == 0 || month == 0 || day == 0
            ? DateTimeOffset.MinValue
            : new(new DateTime(year, month, day, hour, minutes, seconds), TimeSpan.FromHours(offset));
    }
}
