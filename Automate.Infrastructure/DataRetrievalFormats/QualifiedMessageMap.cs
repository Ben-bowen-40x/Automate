using CsvHelper.Configuration;
using Automate.Domain.ValueObjects;
using CsvHelper.Configuration.Attributes;
using Automate.Translation.QualifiedMessageTranslate;
using Automate.Translation.MessageTranslate;

namespace Automate.Infrastructure.DataRetrievalFormats;

internal class QualifiedMessageMap : ClassMap<QualifiedMessageRecord>, IQualifiedMessageTranslate
{
    public const string PhoneName = "Phone Number";
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
        Map(m => m.Message.Date.UtcDateTime).Index(index++).Name(DateName);
        Map(m => m.Message.Contents).Index(index++).Name(ContentsName);
        Map(m => m.Message.Source).Index(index++).Name(SourceName);

        // Lead info
        Map(m => m.Billable).Index(index++).Name(ImLeadName);
        Map(m => m.IsSalesLead).Index(index++).Name(SalesLeadName);

        // Customer info
        Map(m => m.Customer.CustomerId).Index(index++).Name(CustomerIdName);
        Map(m => m.Customer.SubscriptionActive).Index(index++).Name(SubscriptionActiveName);
        Map(m => m.Customer.Date.UtcDateTime).Index(index++).Name(CustomerStartName);
        Map(m => m.Customer.CustomerCancelDate.UtcDateTime).Index(index++).Name(CustomerCancelName);

        // Subscription info
        Map(m => m.Customer.SubscriptionId).Index(index++).Name(SubscriptionIdName);
        Map(m => m.Customer.InitialCompleted).Index(index++).Name(SubCompletedInitialName);
        Map(m => m.Customer.ContractValue).Index(index++).Name(ContractValueName);
        Map(m => m.Customer.SubscriptionStartDate.UtcDateTime).Index(index++).Name(SubStartDateName);
        Map(m => m.Customer.SubscriptionCancelDate.UtcDateTime).Index(index++).Name(SubCancelDateName);
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
    public DateTime CustomerStartDate { get; set; }

    [Name(CustomerCancelName)]
    public DateTime CustomerCancelDate { get; set; }

    [Name(SubscriptionIdName)]
    public long SubId { get; set; }

    [Name(SubCompletedInitialName)]
    public bool CompletedInitial { get; set; }

    [Name(ContractValueName)]
    public double ContractValue { get; set; }

    [Name(SubStartDateName)]
    public DateTime SubStartDate { get; set; }

    [Name(SubCancelDateName)]
    public DateTime SubCancelDate { get; set; }

    [Name(SellersName)]
    public string? Sellers { get; set; }
    public IMessage Convert<IMsgDTONumberLong, IMessage>()
    {
        Translation.MessageTranslate.IMsgDTONumberLong that = this;
        return (IMessage)that.Convert();
    }
}