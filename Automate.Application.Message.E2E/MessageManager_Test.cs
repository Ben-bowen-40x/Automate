using Automate.Application.MessageAnalysis;
using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.MessageLeadsService;
using Automate.Infrastructure.MessageLeadsService.CsvMaps;
using Automate.Infrastructure.ReportingService;

namespace Automate.Application.Message.E2E;

public class MessageManager_Test(IDwhSettings settings)
{
    private const string MsgAnalysis = @".info\MessageAnalysis";
    private const string MsgLeads = "MessagesToAnalyze.csv";
    private const string CctLeads = "PNContactForms.csv";
    [
        Theory,
        InlineData(MsgLeads),
        InlineData(CctLeads),
    ]
    public void TextManager_ProperlyExecutes(string fileName)
    {
        // Log this test instance
        object sender = new MessageManager_Test(settings);
        string member = nameof(TextManager_ProperlyExecutes);
        StringLogger.NewLog(DateTime.Now, sender, member, "Start Test");

        // Assemble
        MessageAnalysisManager manager = new(new MessageService(settings), new ReportServiceSingleton());

        // Act
        var result =
            fileName switch
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
