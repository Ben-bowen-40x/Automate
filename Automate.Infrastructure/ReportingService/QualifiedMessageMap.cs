using CsvHelper.Configuration;
using Automate.Domain.ValueObjects;

namespace Automate.Infrastructure.ReportingService;

internal class QualifiedMessageMap : ClassMap<QualifiedMessageRecord>
{
    public const string Phone = "Phone Number";
    public const string Date = "Date Of Message";
    public const string Contents = "Message Contents";
    public const string Source = "Message Source";
    public const string ImLead = "IM Lead";
    public const string SalesLead = "Sales Lead";
    public const string CustomerId = "Customer ID (most likely)";
    public const string SubscriptionActive = "Subscription is active";
    public const string CustomerStart = "Customer record start date";
    public const string CustomerCancel = "Customer cancel date";
    public const string SubscriptionId = "Subscription ID (most likely)";
    public const string SubCompletedInitial = "Completed Initial";
    public const string ContractValue = "Contract Value";
    public const string SubStartDate = "Subscription start date";
    public const string SubCancelDate = "Subscription cancel date";
    public const string Sellers = "Sellers";
    public QualifiedMessageMap()
    {
        int index = 0;

        // Text Info
        Map(m => m.Message.Number.Number).Index(index++).Name(Phone);
        Map(m => m.Message.Date.UtcDateTime).Index(index++).Name(Date);
        Map(m => m.Message.Contents).Index(index++).Name(Contents);
        Map(m => m.Message.Source).Index(index++).Name(Source);

        // Lead info
        Map(m => m.Billable).Index(index++).Name(ImLead);
        Map(m => m.IsSalesLead).Index(index++).Name(SalesLead);

        // Customer info
        Map(m => m.Customer.CustomerId).Index(index++).Name(CustomerId);
        Map(m => m.Customer.SubscriptionActive).Index(index++).Name(SubscriptionActive);
        Map(m => m.Customer.Date.UtcDateTime).Index(index++).Name(CustomerStart);
        Map(m => m.Customer.CustomerCancelDate.UtcDateTime).Index(index++).Name(CustomerCancel);

        // Subscription info
        Map(m => m.Customer.SubscriptionId).Index(index++).Name(SubscriptionId);
        Map(m => m.Customer.InitialCompleted).Index(index++).Name(SubCompletedInitial);
        Map(m => m.Customer.ContractValue).Index(index++).Name(ContractValue);
        Map(m => m.Customer.SubscriptionStartDate.UtcDateTime).Index(index++).Name(SubStartDate);
        Map(m => m.Customer.SubscriptionCancelDate.UtcDateTime).Index(index++).Name(SubCancelDate);
        Map(m => m.Customer.Sellers).Index(index++).Name(Sellers);
    }
}