using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.ReportingService;
using CsvHelper.Configuration.Attributes;
namespace Automate.Infrastructure.MessageLeadsReportService;
internal class MessageReportMap : IMessageConvert
{
    [Name(QualifiedMessageMap.Phone)]
    public long Number { get; set; }
    [Name(QualifiedMessageMap.Date)]
    public DateTime Date { get; set; }
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
    public IMessage ConvertToMessage()
    {
        // Convert phone number
        PhoneNumber num = new(Number);

        // convert date 
        DateTime date = Date;

        // Convert Contents
        string content = Contents is null || Contents == string.Empty 
            ? string.Empty 
            : Contents;

        // Convert Source
        string source = Source is null || Source == string.Empty 
            ? string.Empty 
            : Source;

        // Return result
        return new Message(num, date, content, source);
    }
    public QualifiedMessageRecord ConvertToQualifiedRecord()
    {
        // Retrieve message info from the data
        IMessage message = ConvertToMessage();

        // Convert sellers
        string sellers = Sellers is null || Sellers == string.Empty
            ? string.Empty
            : Sellers;

        // Fix dates, which are in UTC already
        DateTime subStartDate = new(SubStartDate.Ticks, DateTimeKind.Utc);
        DateTime custCxlDate = new(CustomerCancelDate.Ticks, DateTimeKind.Utc);
        DateTime subCancelDate = new(SubCancelDate.Ticks, DateTimeKind.Utc);


        // Retrieve customer info from the data
        ICustomerSubscription customer = new CustomerSubscription(CustomerID, SubId, Date, new(subStartDate), new(Number), new(0), new(custCxlDate), new(subCancelDate), SubIsActive, SubIsActive, CompletedInitial, ContractValue, sellers);

        return new QualifiedMessageRecord(message, customer, ImLead, SalesLead);
    }
}
