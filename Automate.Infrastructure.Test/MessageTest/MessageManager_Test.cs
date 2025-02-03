using Automate.Application.MessageAnalysis;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.MessageLeadsService;
using Automate.Infrastructure.ReportingService;
using Automate.Infrastructure.Test.DiscrepancyTest;
using Automate.Infrastructure.Test.TestConfigurations;

namespace Automate.Infrastructure.Test.MessageTest;

public class MessageManager_Test
{
    public MessageManager_Test()
    {
        _settings = new InfraTestConfiguration().TestSettings;
    }
    private readonly IDwhTestSettings _settings;
    private const string MsgAnalysis = @".info\MessageAnalysis";
    private const string MsgLeads = "MessagesToAnalyze.csv";
    private const string CctLeads = "PNContactForms.csv";
    [
        Theory,
        InlineData(MsgLeads),
        //InlineData(CctLeads),
    ]
    public void TextManager_ProperlyExecutes(string fileName)
    {
        // Log this test instance
        object sender = new MessageManager_Test();
        string member = nameof(TextManager_ProperlyExecutes);
        StringLogger.NewLog(DateTime.Now, sender, member, "Start Test");

        // Assemble
        MessageAnalysisManager manager = new(new MessageService(_settings), new ReportServiceSingleton());

        // Act
        var result = fileName switch
        {
            MsgLeads => manager.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>("", "", "", "", ""),
            CctLeads => manager.Manage<SplitDateMountainOffsetMsgCol>("", "", "", "", ""),
            _ => manager.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>("", "", "", "", "")
        };
        StringLogger.ProduceLog(DateTime.Now, sender, member, $"End Test");

        // Assert
        Assert.True(result.IsSuccess);
    }
}
