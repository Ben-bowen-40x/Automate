using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.Test.TestConfigurations;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.Test.CsvTests;

public class CsvAppend_Test
{
    public CsvAppend_Test()
    {
        TestFileLocation1 = new(new InfraTestConfiguration().TestSettings.CsvAppendTestFile!);
    }
    public FileInfo TestFileLocation1 { get; }

    #region CsvAppendDoesNotHaveStackOverflowIssues
    [
        Theory,
        InlineData("This is column 1", "This is Colmn 2", "This is column 3"),
        InlineData("Thi is column 1", "This is Column 2", "This is column 3"),
        InlineData("This s column 1", "This is Column 2", "This is column 3"),
        InlineData("This iscolumn 1", "This is Column 2", "This is column 3"),
        InlineData("This is clumn 1", "This is Column 2", "This is column 3"),
        InlineData("This is column 1", "This is Clumn 2", "This is column 3"),
        InlineData("This is colun 1", "This is Column 2", "This is column 3"),
        InlineData("This is column1", "his is Column 2", "This is column 3"),
        InlineData("This is column 1", "his is Column 2", "This is column 3"),
        InlineData("This is column 1", "Ths is Column 2", "This is column 3"),
        InlineData("This is column 1", "Thisis Column 2", "This is column 3"),
    ]
    public void CsvAppendDoesNotHaveStackOverflowIssues(string contentToAppend1, string contentToAppend2, string contentToAppend3)
    {
        // If the file doesn't exist, create it 
        if (!File.Exists(TestFileLocation1.FullName))
        {
            File.WriteAllText(TestFileLocation1.FullName, $"{CsvAppendTestColumns.c1},{CsvAppendTestColumns.c2},{CsvAppendTestColumns.c3}\n");
        }

        // Translate input to objects
        List<CsvAppendTestColumns> unparsed =
        [
            new()
            {
                Col1 = contentToAppend1,
                Col2 = contentToAppend2,
                Col3 = contentToAppend3,
            }
        ];

        // Append to the file
        CsvService.Append<CsvAppendTestColumns, CsvAppend_TestMap>(TestFileLocation1, unparsed);
    }
    #endregion

    #region CsvAppendDoesNotHaveStackOverflowIssues_IOErrortest
    [
            Theory,
            InlineData("This is column 1", "This is Column 2", "This is column 3"),
            InlineData("Thi is column 1", "This is Column 2", "This is column 3"),
            InlineData("This s column 1", "This is Column 2", "This is column 3"),
            InlineData("This iscolumn 1", "This is Column 2", "This is column 3"),
            InlineData("This is clumn 1", "This is Column 2", "This is column 3"),
            InlineData("This is column 1", "This is Clumn 2", "This is column 3"),
            InlineData("This is colun 1", "This is Column 2", "This is column 3"),
            InlineData("This is column1", "his is Column 2", "This is column 3"),
            InlineData("This is column 1", "his is Column 2", "This is column 3"),
            InlineData("This is column 1", "Ths is Column 2", "This is column 3"),
            InlineData("This is column 1", "Thisis Column 2", "This is column 3"),
    ]
    public void CsvAppendDoesNotHaveStackOverflowIssues_IOErrortest(string contentToAppend1, string contentToAppend2, string contentToAppend3)
    {
        // If the file doesn't exist, create it 
        if (!File.Exists(TestFileLocation1.FullName))
        {
            File.WriteAllText(TestFileLocation1.FullName, $"{CsvAppendTestColumns.c1},{CsvAppendTestColumns.c2},{CsvAppendTestColumns.c3}\n");
        }

        // Translate input to objects
        List<CsvAppendTestColumns> unparsed =
        [
            new()
            {
                Col1 = contentToAppend1,
                Col2 = contentToAppend2,
                Col3 = contentToAppend3,
            }
        ];
        // Read from the file
        var contents = CsvService.Parse<CsvAppendTestColumns>(TestFileLocation1);

        // Append to the file
        CsvService.Append<CsvAppendTestColumns, CsvAppend_TestMap>(TestFileLocation1, unparsed);
    }
    #endregion
}


#region Necessary objects -- Do Not separate into another file, please
internal class CsvAppend_TestMap : ClassMap<CsvAppendTestColumns>
{
    public CsvAppend_TestMap()
    {
        int index = 0;
        Map(m => m.Col1).Index(index++).Name(CsvAppendTestColumns.c1);
        Map(m => m.Col1).Index(index++).Name(CsvAppendTestColumns.c2);
        Map(m => m.Col1).Index(index++).Name(CsvAppendTestColumns.c3);
    }
}
internal class CsvAppendTestColumns
{
    public const string c1 = "Column1";
    public const string c2 = "Column2";
    public const string c3 = "Column3";
    [Name(c1)]
    public string? Col1 { get; set; }
    [Name(c2)]
    public string? Col2 { get; set; }
    [Name(c2)]
    public string? Col3 { get; set; }
}
#endregion