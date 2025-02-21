using Automate.Domain.ValueObjects;
using Automate.Translation.CallTranslate;
using Automate.Translation.PhoneNumTranslate;
using Automate.Translation.ValueObjectTranslate;

namespace Automate.Translation.CallTranslate;

public static class CallInterfaceTranslate
{
    /// <summary>
    /// Extension Method Translates from <see cref="ICallDateTimeInUTC"/> to <see cref="ICallRecord"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns><see cref="ICallRecord"/> with all the proper values, based on the given <paramref name="entity"/></returns>
    // Tests are unnecessary because this method's components are tested elsewhere
    public static ICallRecord Translate(this ICallDateTimeInUTC entity)
    {
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        TimeSpan timeZone = ConvertPrimitive.ConvertTimeSpan(entity.TimeZone);
        DateTimeOffset date = ConvertPrimitive.ConvertDateTimeOffset(entity.Date, timeZone, DateDefault.Min);
        ICallRecord record = new MessageCallRecord(entity.Number, date, billable);
        return record;
    }

    /// <summary>
    /// Extension method translates from <see cref="ICallType"/> to <see cref="ICallRecord"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns><see cref="ICallRecord"/> with all the proper values, based on the given <paramref name="entity"/></returns>
    // Tests are unnecessary because this method's components are tested elsewhere
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
    /// <returns><see cref="ICallRecord"/> with all the proper values, based on the given <paramref name="entity"/></returns>
    // Tests are unnecessary because this method's components are tested elsewhere
    public static ICallRecord Translate(this ICallBillableStrNumberType entity)
    {
        PhoneNumber number = entity.Number.Translate();
        bool billable = ConvertPrimitive.ConvertBool(entity.Billable);
        ICallRecord result = new MessageCallRecord(number, entity.Date, billable);
        return result;
    }
}
