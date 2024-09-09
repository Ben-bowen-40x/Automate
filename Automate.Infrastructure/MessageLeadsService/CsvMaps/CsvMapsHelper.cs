namespace Automate.Infrastructure.MessageLeadsService.CsvMaps;

internal class CsvMapsHelper
{
    internal static string ContentsJoined(string contents)
    {
        string str = string.Join('|', contents.Split(',', '"'));
        return string.Join(" | ", str.Split('\n', '\r'));
    }
}

