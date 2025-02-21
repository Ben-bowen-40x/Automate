using System.Text.RegularExpressions;

namespace Automate.Translation.ValueObjectsTranslations;

internal partial class TSH // Translate Service Helper = TSH
{
    internal static string ContentsJoined(string contents)
    {
        string str = ReplaceCsvAwkward(contents, '|');
        string result = BarSpace().Replace(str, "| ");
        return result;
    }

    internal readonly static char[] chars = { ',', '"', '\n', '\r' };
    internal static string ReplaceCsvAwkward(string input, char joiner)
    {
        return ReplaceCsvAwkward(input, $"{joiner}");
    }
    internal static string ReplaceCsvAwkward(string input, string? joiner)
    {
        var split = input.Split(chars);
        string str = chars.Any(input.Contains)
            ? string.Join(joiner, split)
            : input;
        return str;
    }

    [GeneratedRegex(@"(\s*\|\s*)+")]
    private static partial Regex BarSpace();
}
