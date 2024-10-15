namespace Automate.Domain.ValueObjects;

public class PhoneNumber
{
    #region Public
    public override string ToString()
    {
        return $"Phone Number: {Number}";
    }

    public bool IsDefault { get; private set; }

    public static long Default => 0;

    public long Number { get; set; }

    public PhoneNumber(PhoneNumber number)
    {
        Number = number.Number;
        IsDefault = number.IsDefault;
    }

    public PhoneNumber(long number)
    {
        Number = ValidateNumericalInput(number);
    }

    public PhoneNumber(string number)
    {
        Number = NullCheck(number);
    }

    public static bool TryParse(string? number, out PhoneNumber result)
    {
        if (number is null)
        {
            result = new(0);
            return false;
        }
        try
        {
            result = new(number!);
            return true;
        }
        catch
        {
            result = new(0);
            return false; }
    }
    #endregion

    #region Internal
    internal long NullCheck(string number)
    {
        if (number is null || number == string.Empty)
            return Default;
        else
            return ValidateStringInput(number);
    }

    internal long ValidateStringInput(string number)
    {
        // Place intermediate variables here for debugging purposes
        if (long.TryParse(number, out long conversion))
        {
            long result = ValidateNumericalInput(conversion);
            return result;
        }
        else
        {
            string split = RemoveNondigitChars(number);
            long noCountry = StrToLong(split);
            long result = ValidateNumericalInput(noCountry);
            return result;
        }
    }

    internal static string RemoveNondigitChars(string number)
    {
        string spl = string.Join(string.Empty, number.Split('+'));
        string split = string.Join(string.Empty, spl.Split('('));
        string split2 = string.Join(string.Empty, split.Split(')'));
        string split3 = string.Join(string.Empty, split2.Split(' '));
        string split4 = string.Join(string.Empty, split3.Split('-'));
        return split4;
    }

    internal long ValidateNumericalInput(long number)
    {
        IsDefault = number == 0 || number == 1111111111;
        return number switch
        {
            // Remove the country code, if necessary
            long n when $"{n}".Length > 10 => StrToLong($"{number}"),
            _ => number
        };
    }

    internal static long StrToLong(string number)
    {
        if (number.Length < 10)
            return Default;
        if (long.TryParse(number[^10..], out long result))
            return result;
        return Default;
    }
    #endregion
}
