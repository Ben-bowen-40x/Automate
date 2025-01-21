using Automate.Domain.ValueObjects;

namespace Automate.Translation.InfrastructureInterfaces.Message;

public interface IMsgStrDateTimeOffset : IMessage_Number_Str, IMessage_DateTimeOffset, IMessage_Contents_Str, IMessage_Source_Str { }
public interface IMsgTimeStr : IConvert, IMessage_Number_Str, IMessage_Date_Str, IMessage_Time_Str, IMessage_Contents_Str, IMessage_Source_Str { }
public interface IMsgNoTimeStr : IConvert, IMessage_Number_Str, IMessage_Date_Str, IMessage_Contents_Str, IMessage_Source_Str { }
public interface IMessage_Number_Str // Number as nullable string
{
    string? Number { get; set; }
}
public interface IMessage_DateTimeOffset // Date as DateTimeOffset
{
    DateTimeOffset Date { get; set; }
}
public interface IMessage_Date_Str // Date as nullable string
{
    string? Date { get; set; }
}
public interface IMessage_Time_Str // Time as nullable string
{
    string? Time { get; set; }
}
public interface IMessage_Contents_Str // Contents as nullable string
{
    string? Contents { get; set; }
}
public interface IMessage_Source_Str // Source as nullable string
{
    string? Source { get; set; }
}