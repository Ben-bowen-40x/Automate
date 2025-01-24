using System.Text.RegularExpressions;

namespace Automate.Translation.ValueObjectsTranslations;

internal partial class TSH // Translation Service Helper = TSH
{
    internal static string ContentsJoined(string contents)
    {
        string str = string.Join('|', contents.Split(',', '"', '\n', '\r'));
        string result = BarSpace().Replace(str, "| ");
        return result;
    }

    [GeneratedRegex(@"(\s*\|\s*)+")]
    private static partial Regex BarSpace();
}