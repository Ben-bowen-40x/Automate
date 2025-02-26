using CsvHelper.Configuration;
using Automate.Domain.ValueObjects;
using CsvHelper.Configuration.Attributes;
using Automate.Translation.QualifiedMessageTranslate;
using Automate.Translation.MessageTranslate;
using Automate.Translation.PhoneNumTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

internal class QualifiedMessageMap : ClassMap<QualifiedMessageRecord>, IQualifiedMessageTranslate
{
    public const string PhoneName = "Phone Number";
    public const string Phone1Name = "Customer Phone1";
    public const string Phone2Name = "Customer Phone2";
    public const string DateName = "Date Of Message";
    public const string ContentsName = "Message Contents";
    public const string SourceName = "Message Source";
    public const string ImLeadName = "IM Lead";
    public const string SalesLeadName = "Sales Lead";
    public const string CustomerIdName = "Customer ID (most likely)";
    public const string SubscriptionActiveName = "Subscription is active";
    public const string CustomerStartName = "Customer record start date";
    public const string CustomerCancelName = "Customer cancel date";
    public const string SubscriptionIdName = "Subscription ID (most likely)";
    public const string SubCompletedInitialName = "Completed Initial";
    public const string ContractValueName = "Contract Value";
    public const string SubStartDateName = "Subscription start date";
    public const string SubCancelDateName = "Subscription cancel date";
    public const string SellersName = "Sellers";
    public QualifiedMessageMap()
    {
        int index = 0;
        // Text Info
        Map(m => m.Message.Number.Number).Index(index++).Name(PhoneName);
        Map(m => m.Message.Date).Index(index++).Name(DateName);
        Map(m => m.Message.Contents).Index(index++).Name(ContentsName);
        Map(m => m.Message.Source).Index(index++).Name(SourceName);

        // Lead info
        Map(m => m.Billable).Index(index++).Name(ImLeadName);
        Map(m => m.IsSalesLead).Index(index++).Name(SalesLeadName);

        // Customer info
        Map(m => m.Customer.CustomerId).Index(index++).Name(CustomerIdName);
        //Map(m => m.Customer.Number.Number).Index(index++).Name(Phone1Name); // Ready for when we wish to add phone numbers to the reports
        //Map(m => m.Customer.Number2.Number).Index(index++).Name(Phone2Name); // Readon for when we wish to add phone numbers to the reports
        Map(m => m.Customer.SubscriptionActive).Index(index++).Name(SubscriptionActiveName);
        Map(m => m.Customer.Date).Index(index++).Name(CustomerStartName);
        Map(m => m.Customer.CustomerCancelDate).Index(index++).Name(CustomerCancelName);

        // Subscription info
        Map(m => m.Customer.SubscriptionId).Index(index++).Name(SubscriptionIdName);
        Map(m => m.Customer.InitialCompleted).Index(index++).Name(SubCompletedInitialName);
        Map(m => m.Customer.ContractValue).Index(index++).Name(ContractValueName);
        Map(m => m.Customer.SubscriptionStartDate).Index(index++).Name(SubStartDateName);
        Map(m => m.Customer.SubscriptionCancelDate).Index(index++).Name(SubCancelDateName);
        Map(m => m.Customer.Sellers).Index(index++).Name(SellersName);
    }
    [Name(PhoneName)]
    public long Number { get; set; }

    [Name(DateName)]
    public DateTimeOffset Date { get; set; }

    [Name(ContentsName)]
    public string? Contents { get; set; }

    [Name(SourceName)]
    public string? Source { get; set; }

    [Name(ImLeadName)]
    public bool ImLead { get; set; }

    [Name(SalesLeadName)]
    public bool SalesLead { get; set; }

    [Name(CustomerIdName)]
    public long CustomerID { get; set; }

    [Name(SubscriptionActiveName)]
    public bool SubIsActive { get; set; }

    [Name(CustomerStartName)]
    public DateTimeOffset CustomerStartDate { get; set; }

    [Name(CustomerCancelName)]
    public DateTimeOffset CustomerCancelDate { get; set; }

    [Name(SubscriptionIdName)]
    public long SubId { get; set; }

    [Name(SubCompletedInitialName)]
    public bool CompletedInitial { get; set; }

    [Name(ContractValueName)]
    public double ContractValue { get; set; }

    [Name(SubStartDateName)]
    public DateTimeOffset SubStartDate { get; set; }

    [Name(SubCancelDateName)]
    public DateTimeOffset SubCancelDate { get; set; }

    [Name(SellersName)]
    public string? Sellers { get; set; }

    /// <summary>
    /// Currently, this map does not read the phone numbers because it doesn't save phone numbers from the customer
    /// <para>For this reason, the phone numbers are defaulting to <see cref="Number"/></para>
    /// <para>If we want to save the phone numbers in the map configuration in the future, we decorate these properties <see cref="Phone1"/> and <see cref="Phone2"/> with the same attributes as the others in <see cref="QualifiedMessageMap"/></para>
    /// <para>Please ensure this the <see cref="Phone1Name"/> and <see cref="Phone2Name"/> fill in the property values for their respective properties, this <see cref="_phone"/> is deleted, and <see cref="Phone1"/> and <see cref="Phon2"/> use default setters and getters</para>
    /// <para>At such time as these become attribute-decorated properties as described above, this summary comment will be unnecessary</para>
    /// </summary>
    //[Name(Phone1Name)]/*
    [Ignore]//*/
    public long Phone1 { get => _phone ??= Number; set => _phone = Number; }

    /// <summary>
    /// See comments to <see cref="Phone1"/>
    /// </summary>
    //[Name(Phone2Name)]/*
    [Ignore]//*/
    public long Phone2 { get => _phone ??= Number; set => _phone = Number; }
    private long? _phone;

    public IMessage Convert<IMsgDTONumberLong, IMessage>()
    {
        Translation.MessageTranslate.IMsgDTONumberLong @this = this;
        return (IMessage)@this.Translate();
    }
}