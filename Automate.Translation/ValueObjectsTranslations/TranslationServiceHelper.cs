namespace Automate.Translation;

internal class TSH // Translation Service Helper = TSH
{
    internal static string ContentsJoined(string contents)
    {
        string str = string.Join('|', contents.Split(',', '"'));
        return string.Join(" | ", str.Split('\n', '\r'));
    }
}