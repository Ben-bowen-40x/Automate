using Microsoft.Extensions.DependencyInjection;
using Automate.Application.MessageReportAnalysis;
using Automate.Application.MessageAnalysis;
using CSharpFunctionalExtensions;
using Automate.Infrastructure.DataRetrievalFormats;

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
    Leased,
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
        Leased,
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
            (MessageCsvType.MetaForm, true, true) => appender.Manage<UnifiedDateUtc_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.MetaForm, true, false) => appender.Manage<UnifiedDateUtc_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.MetaForm, false, true) => generator.Manage<UnifiedDateUtc_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.MetaForm, false, false) => generator.Manage<UnifiedDateUtc_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Libacion
            (MessageCsvType.Libacion, true, true) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.Libacion, true, false) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Libacion, false, true) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.Libacion, false, false) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageCsvType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Leased
            (MessageCsvType.Leased, true, true) => appender.Manage<LeasedMessage>(MessageCsvType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, days),
            (MessageCsvType.Leased, true, false) => appender.Manage<LeasedMessage>(MessageCsvType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Leased, false, true) => generator.Manage<LeasedMessage>(MessageCsvType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, days),
            (MessageCsvType.Leased, false, false) => generator.Manage<LeasedMessage>(MessageCsvType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Default
            _ => throw new Exception($"There is no case where the input can be executed. Here is the input:\n{nameof(append)}: {append}\n{nameof(service)}: {service}\n{nameof(messageLocation)}: {messageLocation}\n{nameof(callQueryLocation)}: {callQueryLocation}\n{nameof(customerQueryLocation)}: {customerQueryLocation}\n{nameof(reportLocation)}: {reportLocation}\n{nameof(messageType)}: {messageType}\n{nameof(truncateReport)}: {truncateReport}\n{nameof(truncate)}: {truncate}\n{nameof(days)}: {days}")
        };
    }
}
