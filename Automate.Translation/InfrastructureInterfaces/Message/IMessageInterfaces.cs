using Automate.Application.InfrastructureInterfaces;
using Automate.Domain.ValueObjects;
using Automate.Translation.DateTimeConvertService;

namespace Automate.Translation.InfrastructureInterfaces.Message;

public interface IMsgStrDateTimeOffset : IConvert, IMessage_Number_Str, IDatedRecord, IMessage_Contents_Str, IMessage_Source_Str { }
public interface IMsgZoneEnumStr : IConvert, IMessage_Number_Str, IMessage_DateTime_Str, IMessage_Contents_Str, IMessage_Source_Str, IMessage_TimeZoneEnum { }
public interface IMsgZoneStr : IMessage_NumberCompatible, IMessage_Number_Long, IMessage_BillableStr, IMessage_NullableDateTime, IMessage_TimeZoneStr { }
public interface IMsgNoTimeStr : IConvert, IMessage_Number_Str, IMessage_Date_Str, IMessage_Contents_Str, IMessage_Source_Str { }
public interface IMsgDTOStr : IMsgNoTimeStr { }
public interface IMsgDTOStrNonEmptySource : IMsgDTOStrIsolateSource { } // Why are we doing this, you ask? Well because they are the same thing on this level, yes, but they are different on the conversion level
public interface IMsgDTOStrIsolateSource : IMsgDTOStr, IMessage_IsolateSourceComponent { }
public interface IMessage_DateTime_Str : IMessage_Date_Str, IMessage_Time_Str { }
public interface IMessage_Number_Str // NumberStr as nullable string
{
    string? NumberStr { get; set; }
}
public interface IMessage_Number_Long // NumberLong as nullable string
{
    long NumberLong { get; set; }
}
public interface IMessage_NumberCompatible : IPhoneNumberCompatible { } // NumberLong as PhoneNumber
public interface IMessage_Date_Str // Date as nullable string
{
    string? DateStr { get; set; }
}
public interface IMessage_NullableDateTime // Date as nullable DateTime
{
    DateTime? Date { get; set; }
}
public interface IMessage_BillableStr // Billability as string
{
    string? BillableStr { get; set; }
}
public interface IMessage_BillableBool // Billability as boolean
{
    bool Billable { get; set; }
}
public interface IMessage_Time_Str // TimeStr as nullable string
{
    string? TimeStr { get; set; }
}
public interface IMessage_Contents_Str // Contents as nullable string
{
    string? Contents { get; set; }
}
public interface IMessage_Source_Str // Source as nullable string
{
    string? Source { get; set; }
}
public interface IMessage_TimeZoneEnum
{
    TimeZoneEnum TimeZone { get; }
}
public interface IMessage_TimeZoneStr
{
    string? TimeZoneStr { get; }
}
public interface IMessage_IsolateSourceComponent
{
    SourceComponent Separator { get; }
}
