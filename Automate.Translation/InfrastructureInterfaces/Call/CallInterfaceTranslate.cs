using Automate.Domain.ValueObjects;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.InfrastructureInterfaces.Call;

public static class CallInterfaceTranslate
{
    /// <summary>
    /// Extension Method Translates <paramref name="entity"/> from <see cref="ICallZoneStr"/> to <see cref="IMessage"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICallRecord Convert(this ICallZoneStr entity)
    {
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        TimeSpan timeZone = ConvertPrimitive.ConvertTimeSpan(entity.TimeZone);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.Date, timeZone, DateTimeDefaults.Min);
        ICallRecord record = new MessageCallRecord(entity.Number, date, billable);
        return record;
    }
}
