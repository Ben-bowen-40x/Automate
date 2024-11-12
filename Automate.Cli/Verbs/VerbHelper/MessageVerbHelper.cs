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
        """;
    internal static Result<FileInfo> Execute(bool append, IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
    {
        var generator = service.GetRequiredService<IMessageAnalysisManager>();
        var appender = service.GetRequiredService<IMessageAnalysisReportManager>();
        return (messageType, append) switch
        {
            // Pan
            (MessageCsvType.Pan, true) => appender.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Pan, false) => generator.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // GAdsLeaf
            (MessageCsvType.GAdsLeaf, true) => appender
                .Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.GAdsLeaf, false) => generator
                .Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // GAdsLeafRepo
            (MessageCsvType.GAdsLeafRepo, true) => appender.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.GAdsLeafRepo, false) => generator.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // ManualWebForm
            (MessageCsvType.ManualWebForm, true) => appender.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.ManualWebForm, false) => generator.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Leaf
            (MessageCsvType.Leaf, true) => appender.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.Leaf, false) => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // LeafRepo
            (MessageCsvType.LeafRepo, true) => appender.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.LeafRepo, false) => generator.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Meta
            (MessageCsvType.MetaForm, true) => appender.Manage<UnifiedDate_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
            (MessageCsvType.MetaForm, false) => generator.Manage<UnifiedDate_SplitPhone>(MessageCsvType.MetaForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),

            // Default
            _ => generator.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation)
        };
        // Old Code -- This is here for verification purposes and debugging only
        // TODO: delete old code here, once everything is verified
        /* return append
           ? Append(service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType)
           : GenerateNew(service, messageLocation, callQueryLocation, customerQueryLocation, reportLocation, messageType);

       // Locals
       static Result<FileInfo> GenerateNew(IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
       {
           var m = service.GetRequiredService<IMessageAnalysisManager>();
           return messageType switch
           {
               MessageCsvType.Pan => m.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.GAdsLeaf => m.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.GAdsLeafRepo => m.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.ManualWebForm => m.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.Leaf => m.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.LeafRepo => m.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               _ => m.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
           };
       }

       static Result<FileInfo> Append(IServiceProvider service, string messageLocation, string callQueryLocation, string customerQueryLocation, string reportLocation, MessageCsvType messageType)
       {
           IMessageAnalysisReportManager manage = service.GetRequiredService<IMessageAnalysisReportManager>();
           return messageType switch
           {
               MessageCsvType.Pan => manage.Manage<SplitDateMountainOffsetMsgCol>(MessageCsvType.Pan.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.GAdsLeaf => manage.Manage<UnifiedDateUnchangedOffset_SeparateGclid_SourceCantBeEmpty_MsgCol>(MessageCsvType.GAdsLeaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.GAdsLeafRepo => manage.Manage<MessageClass>(MessageCsvType.GAdsLeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.ManualWebForm => manage.Manage<NoTimeMsgCol>(MessageCsvType.ManualWebForm.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.Leaf => manage.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               MessageCsvType.LeafRepo => manage.Manage<MessageClass>(MessageCsvType.LeafRepo.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
               _ => manage.Manage<UnifiedDateUnchangedOffset_SeparateGclid_MsgCol>(MessageCsvType.Leaf.ToString(), messageLocation, callQueryLocation, customerQueryLocation, reportLocation),
           };
       }
       //*/
    }
}
