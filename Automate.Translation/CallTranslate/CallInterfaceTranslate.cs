using Automate.Domain.ValueObjects;
using Automate.Translation.CallTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.ValueObjectsTranslations;

namespace Automate.Translation.CallTranslate;

public static class CallInterfaceTranslate
{
    /// <summary>
    /// Extension Method Translates from <see cref="ICallZoneStr"/> to <see cref="ICallRecord"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICallRecord Translate(this ICallZoneStr entity)
    {
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        TimeSpan timeZone = ConvertPrimitive.ConvertTimeSpan(entity.TimeZone);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.Date, timeZone, DateTimeDefaults.Min);
        ICallRecord record = new MessageCallRecord(entity.Number, date, billable);
        return record;
    }

    /// <summary>
    /// Extension method translates from <see cref="ICallType"/> to <see cref="ICallRecord"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICallRecord Translate(this ICallType entity)
    {
        PhoneNumber number = entity.Number.Translate();
        ICallRecord result = new MessageCallRecord(number, entity.Date, entity.Billable);
        return result;
    }

    /// <summary>
    /// Extension methods translates <see cref="ICallBillableStrNumberType"/> to <see cref="ICallRecord"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static ICallRecord Translate(this ICallBillableStrNumberType entity)
    {
        PhoneNumber number = entity.Number.Translate();
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        ICallRecord result = new MessageCallRecord(number, entity.Date, billable);
        return result;
    }
}
