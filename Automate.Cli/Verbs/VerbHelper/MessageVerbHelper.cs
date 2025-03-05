using Microsoft.Extensions.DependencyInjection;
using Automate.Application.MessageReportAnalysis;
using Automate.Application.MessageAnalysis;
using CSharpFunctionalExtensions;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Domain.ValueObjects;

namespace Automate.Cli.Verbs.VerbHelper;
public class MessageVerbHelper
{
    public const string HelpText = "Choose which type of message file you wish to analyze. The following options are case-sensitive: " + MessageTypeText.Text;
    internal static Result<FileInfo> Execute(bool append, IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageType messageType, string truncateReport, bool truncate, int days)
    {
        var generator = service.GetRequiredService<IMessageAnalysisManager>();
        var appender = service.GetRequiredService<IMessageAnalysisReportManager>();
        return (messageType, append, truncate) switch
        {
            // Pan
            (MessageType.Pan, true, true) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Pan, true, false) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.Pan, false, true) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.Pan, false, false) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // GAdsLeaf
            (MessageType.GAdsLeaf, true, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.GAdsLeaf, true, false) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.GAdsLeaf, false, true) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.GAdsLeaf, false, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // GAdsLeafRepo
            (MessageType.GAdsLeafRepo, true, true) => appender.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.GAdsLeafRepo, true, false) => appender.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.GAdsLeafRepo, false, true) => generator.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.GAdsLeafRepo, false, false) => generator.Manage<MessageClass>(MessageType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // ManualWebForm
            (MessageType.ManualWebForm, true, true) => appender.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.ManualWebForm, true, false) => appender.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.ManualWebForm, false, true) => generator.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.ManualWebForm, false, false) => generator.Manage<NoTimeMsgCol>(MessageType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // Leaf
            (MessageType.Leaf, true, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Leaf, true, false) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.Leaf, false, true) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.Leaf, false, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // LeafRepo
            (MessageType.LeafRepo, true, true) => appender.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.LeafRepo, true, false) => appender.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.LeafRepo, false, true) => generator.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.LeafRepo, false, false) => generator.Manage<MessageClass>(MessageType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // Meta
            (MessageType.MetaForm, true, true) => appender.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.MetaForm, true, false) => appender.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.MetaForm, false, true) => generator.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.MetaForm, false, false) => generator.Manage<UnifiedDateUtc_SplitPhone>(MessageType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // Libacion
            (MessageType.Libacion, true, true) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Libacion, true, false) => appender.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.Libacion, false, true) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.Libacion, false, false) => generator.Manage<SplitDateUTCOffsetMsgCol>(MessageType.Libacion.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // Leased
            (MessageType.Leased, true, true) => appender.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncateReport, truncate, messageType, days),
            (MessageType.Leased, true, false) => appender.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),
            (MessageType.Leased, false, true) => generator.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, truncate, messageType, days),
            (MessageType.Leased, false, false) => generator.Manage<LeasedMessage>(MessageType.Leased.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType),

            // Default
            _ => throw new Exception($"There is no case where the input can be executed. Here is the input:\n{nameof(append)}: {append}\n{nameof(service)}: {service}\n{nameof(messageLocation)}: {messageLocation}\n{nameof(callQueryLocation)}: {callQueryLocation}\n{nameof(customerQueryLocation)}: {customerQueryLocation}\n{nameof(reportLocation)}: {reportLocation}\n{nameof(messageType)}: {messageType}\n{nameof(truncateReport)}: {truncateReport}\n{nameof(truncate)}: {truncate}\n{nameof(days)}: {days}")
        };
    }
}
