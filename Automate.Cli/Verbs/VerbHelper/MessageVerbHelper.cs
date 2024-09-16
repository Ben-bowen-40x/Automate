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
}

public class MessageVerbHelper
{
    public const string HelpText = "Choose which type of message file you wish to analyze. The following options are case-sensitive: Pan, Leaf, LeafRepo, ManualWebForm, GAdsLeaf, GAdsLeafRepo";
    internal static Result<FileInfo> Execute(bool append, IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
    {
        return append
            ? Append(service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType)
            : GenerateNew(service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType);

        // Locals
        static Result<FileInfo> GenerateNew(IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
        {
            var m = service.GetRequiredService<IMessageAnalysisManager>();
            return messageType switch
            {
                MessageCsvType.Pan => m.ManageMessageAnalysis<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeaf => m.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeafRepo => m.ManageMessageAnalysis<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.ManualWebForm => m.ManageMessageAnalysis<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.Leaf => m.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.LeafRepo => m.ManageMessageAnalysis<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                _ => m.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            };
        }

        static Result<FileInfo> Append(IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
        {
            IMessageAnalysisReportManager manage = service.GetRequiredService<IMessageAnalysisReportManager>();
            return messageType switch
            {
                MessageCsvType.Pan => manage.ManageMessageAnalysis<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeaf => manage.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.GAdsLeafRepo => manage.ManageMessageAnalysis<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.ManualWebForm => manage.ManageMessageAnalysis<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.Leaf => manage.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                MessageCsvType.LeafRepo => manage.ManageMessageAnalysis<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
                _ => manage.ManageMessageAnalysis<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            };
        }
    }
}
