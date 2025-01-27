using System.Text.RegularExpressions;

namespace Automate.Translation.ValueObjectsTranslations;

internal partial class TSH // Translate Service Helper = TSH
{
    internal static string ContentsJoined(string contents)
    {
        char[] chars = [',', '"', '\n', '\r'];
        string str = chars.Any(contents.Contains)
            ? string.Join('|', contents.Split(chars))
            : contents;
        string result = BarSpace().Replace(str, "| ");
        return result;
    }

    [GeneratedRegex(@"(\s*\|\s*)+")]
    private static partial Regex BarSpace();
}