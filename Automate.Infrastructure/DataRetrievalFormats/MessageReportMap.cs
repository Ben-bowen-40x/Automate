using Automate.Domain.ValueObjects;
using Automate.Translation.MessageTranslate;
using Automate.Translation.QualifiedMessageTranslate;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
namespace Automate.Infrastructure.DataRetrievalFormats;
internal class MessageReportMap : ClassMap<QualifiedMessageRecord>, IQualifiedMessageTranslate
{
    MessageReportMap()
    {
        int index = 0;

        // Text Info
        Map(m => m.Message.Number.Number).Index(index++).Name(QualifiedMessageMap.Phone);
        Map(m => m.Message.Date).Index(index++).Name(QualifiedMessageMap.Date);
        Map(m => m.Message.Contents).Index(index++).Name(QualifiedMessageMap.Contents);
        Map(m => m.Message.Source).Index(index++).Name(QualifiedMessageMap.Source);

        // Lead info
        Map(m => m.Billable).Index(index++).Name(QualifiedMessageMap.ImLead);
        Map(m => m.IsSalesLead).Index(index++).Name(QualifiedMessageMap.SalesLead);

        // Customer info
        Map(m => m.Customer.CustomerId).Index(index++).Name(QualifiedMessageMap.CustomerId);
        Map(m => m.Customer.SubscriptionActive).Index(index++).Name(QualifiedMessageMap.SubscriptionActive);
        Map(m => m.Customer.Date).Index(index++).Name(QualifiedMessageMap.CustomerStart);
        Map(m => m.Customer.CustomerCancelDate).Index(index++).Name(QualifiedMessageMap.CustomerCancel);

        // Subscription info
        Map(m => m.Customer.SubscriptionId).Index(index++).Name(QualifiedMessageMap.SubscriptionId);
        Map(m => m.Customer.InitialCompleted).Index(index++).Name(QualifiedMessageMap.SubCompletedInitial);
        Map(m => m.Customer.ContractValue).Index(index++).Name(QualifiedMessageMap.ContractValue);
        Map(m => m.Customer.SubscriptionStartDate).Index(index++).Name(QualifiedMessageMap.SubStartDate);
        Map(m => m.Customer.SubscriptionCancelDate).Index(index++).Name(QualifiedMessageMap.SubCancelDate);
        Map(m => m.Customer.Sellers).Index(index++).Name(QualifiedMessageMap.Sellers);
    }

    [Name(QualifiedMessageMap.Phone)]
    public long Number { get; set; }

    [Name(QualifiedMessageMap.Date)]
    public DateTimeOffset Date { get; set; }

    [Name(QualifiedMessageMap.Contents)]
    public string? Contents { get; set; }

    [Name(QualifiedMessageMap.Source)]
    public string? Source { get; set; }

    [Name(QualifiedMessageMap.ImLead)]
    public bool ImLead { get; set; }

    [Name(QualifiedMessageMap.SalesLead)]
    public bool SalesLead { get; set; }

    [Name(QualifiedMessageMap.CustomerId)]
    public long CustomerID { get; set; }

    [Name(QualifiedMessageMap.SubscriptionActive)]
    public bool SubIsActive { get; set; }

    [Name(QualifiedMessageMap.CustomerStart)]
    public DateTime CustomerStartDate { get; set; }

    [Name(QualifiedMessageMap.CustomerCancel)]
    public DateTime CustomerCancelDate { get; set; }

    [Name(QualifiedMessageMap.SubscriptionId)]
    public long SubId { get; set; }

    [Name(QualifiedMessageMap.SubCompletedInitial)]
    public bool CompletedInitial { get; set; }

    [Name(QualifiedMessageMap.ContractValue)]
    public double ContractValue { get; set; }

    [Name(QualifiedMessageMap.SubStartDate)]
    public DateTime SubStartDate { get; set; }

    [Name(QualifiedMessageMap.SubCancelDate)]
    public DateTime SubCancelDate { get; set; }

    [Name(QualifiedMessageMap.Sellers)]
    public string? Sellers { get; set; }
    public IMessage Convert<IMsgDTONumberLong, IMessage>()
    {
        Translation.MessageTranslate.IMsgDTONumberLong that = this;
        return (IMessage)that.Convert();
    }
}
