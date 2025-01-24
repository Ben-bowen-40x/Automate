using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.MessageLeadsService;
using Automate.Infrastructure.Test.DiscrepancyTest;
namespace Automate.Infrastructure.Test.MessageTest;

public class MessageService_Test(IDwhTestSettings settings)
{
    private readonly IDwhTestSettings _settings = settings;
    private const string MsgAnalysis = @".info\MessageAnalysis";
    private const string MsgLeads = "MessagesToAnalyze.csv";
    private const string CctLeads = "PNContactForms.csv";
    [
        Theory,
        // Gettexts AND querydb
        InlineData(true, true, MsgLeads),
        // Gettexts NOT querydb
        InlineData(true, false, MsgLeads),
        // NOT gettexts AND querydb
        InlineData(false, true, MsgLeads),
        // NOT gettexts NOT querydb
        InlineData(false, false, MsgLeads),


        // Gettexts AND querydb
        InlineData(true, true, CctLeads),
        // Gettexts NOT querydb
        InlineData(true, false, CctLeads),
        // NOT gettexts AND querydb
        InlineData(false, true, CctLeads),
        // NOT gettexts NOT querydb
        InlineData(false, false, CctLeads),
    ]
    public void MessageService_GetsAllThreeRecordSets(bool getTexts, bool queryDb, string fileName)
    {
        // Assemble
        MessageService service = new(_settings) { QueryDbCalls = queryDb, QueryDbCustomers = queryDb };
        MessageService_Test obj = new(_settings);
        string member = nameof(MessageService_GetsAllThreeRecordSets);
        var fullName = GetFullName.GetMemberName(obj, member);
        var folder = FolderFinder.GetLocalFolder(nameof(Infrastructure), MsgAnalysis);

        // Act
        List<IMessage> msgs = [];
        if (getTexts)
        {
            msgs = fileName switch
            {
                MsgLeads => service.GetMessages<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(folder + fileName),
                CctLeads => service.GetMessages<SplitDateMountainOffsetMsgCol>(folder + fileName),
                _ => service.GetMessages<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(folder + fileName)
            };

            // This has to be included because getting messages changes query db
            service.QueryDbCalls = queryDb;
            service.QueryDbCustomers = queryDb;
        }
        List<ICallRecord> calls = service.GetCallRecords("");
        List<ICustomerSubscription> customers = service.GetCustomerRecords("");

        // Assert
        if (getTexts)
            Assert.NotNull(msgs);
        Assert.NotNull(calls);
        Assert.NotNull(customers);
    }
}
