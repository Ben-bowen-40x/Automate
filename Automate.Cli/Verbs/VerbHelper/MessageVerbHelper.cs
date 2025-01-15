using Microsoft.Extensions.DependencyInjection;
using Automate.Infrastructure.MessageLeadsService.CsvMaps;
using Automate.Application.MessageReportAnalysis;
using Automate.Application.MessageAnalysis;
using CSharpFunctionalExtensions;

namespace Automate.Cli.Verbs.VerbHelper;

enum MessageCsvType
{
    Pan,
    Leaf,
    LeafRepo,
    ManualWebForm,
    GAdsLeaf,
    GAdsLeafRepo,
    MetaForm,
    Libacion,
}

public class MessageVerbHelper
{
    public const string HelpText = """
        Choose which type of message file you wish to analyze. The following options are case-sensitive: 
        Pan,
        Leaf,
        LeafRepo,
        ManualWebForm,
        GAdsLeaf,
        GAdsLeafRepo,
        MetaForm,
        Libacion,
        """;
    internal static Result<FileInfo> Execute(bool append, IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType, string truncateReport, bool truncate, int days)
    {
        var generator = service.GetRequiredService<IMessageAnalysisManager>();
        var appender = service.GetRequiredService<IMessageAnalysisReportManager>();
        return (messageType, append, truncate) switch
        {
            // Pan
            (MessageCsvType.Pan, true, true) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.Pan, true, false) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Pan, false, true) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.Pan, false, false) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // GAdsLeaf
            (MessageCsvType.GAdsLeaf, true, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.GAdsLeaf, true, false) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.GAdsLeaf, false, true) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.GAdsLeaf, false, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // GAdsLeafRepo
            (MessageCsvType.GAdsLeafRepo, true, true) => appender.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.GAdsLeafRepo, true, false) => appender.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.GAdsLeafRepo, false, true) => generator.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.GAdsLeafRepo, false, false) => generator.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // ManualWebForm
            (MessageCsvType.ManualWebForm, true, true) => appender.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.ManualWebForm, true, false) => appender.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.ManualWebForm, false, true) => generator.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.ManualWebForm, false, false) => generator.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Leaf
            (MessageCsvType.Leaf, true, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.Leaf, true, false) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Leaf, false, true) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.Leaf, false, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // LeafRepo
            (MessageCsvType.LeafRepo, true, true) => appender.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.LeafRepo, true, false) => appender.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.LeafRepo, false, true) => generator.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.LeafRepo, false, false) => generator.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Meta
            (MessageCsvType.MetaForm, true, true) => appender.Manage<UnifiedDate_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.MetaForm, true, false) => appender.Manage<UnifiedDate_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.MetaForm, false, true) => generator.Manage<UnifiedDate_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.MetaForm, false, false) => generator.Manage<UnifiedDate_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Libacion
            (MessageCsvType.Libacion, true, true) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.Libacion, true, false) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Libacion, false, true) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.Libacion, false, false) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Default
            _ => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation)
        };
    }
}
