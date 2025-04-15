using Automate.Infrastructure.CsvManipulationService;
using CsvHelper;
using System.Dynamic;
using System.Globalization;

namespace Automate.Infrastructure.Test.CsvTests;

public class CsvDynamicTest
{
    [Fact]
    public void ReadsCsvFilesDynamically()
    {
        // Act
        using var reader = new StreamReader(Functions.pathtofile.FullName);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        IEnumerable<dynamic> records = csv.GetRecords<dynamic>();

        // Assert
        Assert.NotNull(records);
    }

    [Fact]
    public void ReadsCsvFilesAnonymously()
    {
        // Act
        using var reader = new StringReader(Functions.pathtofile.FullName);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var anonymous = new
        {
            Contents = string.Empty,
            Date = default(DateTimeOffset),
            Number = default(long),
            Source = string.Empty
        };
        var records = csv.GetRecords(anonymous);

        // Assert
        Assert.NotNull(records);
    }

    [Fact]
    public void ConvertsCsvFilesAnonymously()
    {
        // Assemble
        using var reader = new StreamReader(Functions.pathtofile.FullName);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        IEnumerable<dynamic> records = csv.GetRecords<dynamic>();

        // Act
        var result = records.Select(r => ConvertDynamic.Convert(r)).ToList();

        // Assert
        Assert.NotNull(records);
    }

    [Fact]
    public void WritesCsvFilesAnonymously()
    {
        var records = new List<dynamic>();

        dynamic record = new ExpandoObject();
        record.Contents = string.Empty;
        record.Date = DateTimeOffset.MaxValue;
        record.Number = default(long);
        record.Source = string.Empty;
        records.Add(record);

        var result = CsvService.Write(records, Functions.pathtofile);

        // Assert
        Assert.True(result.IsSuccess);
    }
}

#region Necessary local objects. Please do not move to a new file
public static class ConvertDynamic
{
    public static Items Convert(dynamic item)
    {
        var date = DateTimeOffset.Parse(item.Date);
        var num = long.Parse(item.Number);
        return new(item.Contents, date, num, item.Source);
    }
}
public record Items(string Contents, DateTimeOffset Date, long Number, string Source);
#endregion
