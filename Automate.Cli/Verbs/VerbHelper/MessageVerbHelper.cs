using Microsoft.Extensions.DependencyInjection;
using Automate.Infrastructure.MessageLeadsService.CsvMaps;
using Automate.Application.MessageReportAnalysis;
using Automate.Application.MessageAnalysis;
using System;

namespace Automate.Cli.Verbs.VerbHelper;

enum MessageCsvType
{
    Pan,
    Leaf,
    LeafRepo,
    ManualWebForm,
    GAdsLeaf,
    GAdsLeafRepo,
}

public class MessageVerbHelper
{
    public const string HelpText = "Choose which type of message file you wish to analyze. The following options are case-sensitive: Pan, Leaf, LeafRepo, ManualWebForm, GAdsLeaf";
    internal static Dictionary<bool, FileInfo> Execute(bool append, IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
    {
        return append
            ? Append(service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType)
            : GenerateNew(service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType);

        // Locals
        static Dictionary<bool, FileInfo> GenerateNew(IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
        {
            var m = service.GetRequiredService<IMessageAnalysisManager>();
            return messageType switch
            {
                MessageCsvType.Pan => m.ManageMessageAnalysis<SplitDateMountainOffsetMsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeaf => m.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeafRepo => m.ManageMessageAnalysis<MessageClass>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.ManualWebForm => m.ManageMessageAnalysis<NoTimeMsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.Leaf => m.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.LeafRepo => m.ManageMessageAnalysis<MessageClass>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                _ => m.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            };
        }

        static Dictionary<bool, FileInfo> Append(IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
        {
            IMessageAnalysisReportManager manage = service.GetRequiredService<IMessageAnalysisReportManager>();
            return messageType switch
            {
                MessageCsvType.Pan => manage.ManageMessageAnalysis<SplitDateMountainOffsetMsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeaf => manage.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeafRepo => manage.ManageMessageAnalysis<MessageClass>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.ManualWebForm => manage.ManageMessageAnalysis<NoTimeMsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.Leaf => manage.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.LeafRepo => manage.ManageMessageAnalysis<MessageClass>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                _ => manage.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            };
        }
    }
}
