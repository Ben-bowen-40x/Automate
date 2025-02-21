using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Translation.MessageTranslate;
using Automate.Translation.QualifiedMessageTranslate;
using CSharpFunctionalExtensions;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Automate.Infrastructure.Test.MessageTest;

public class MessageReading_Test
{
    private FileInfo TestFile(string location, string file) => FolderFinder.GetLocalFile(nameof(Infrastructure), location, file);

    #region Special Message Can Be Read
    [
        Theory
        // Skip = ""
        ,
        InlineData("LeasedMessagesTest.csv"),
        InlineData("LeasedMessagesTest2.csv"),
    ]
    public void SpecialMessageType_CanBeReadFromTest(string file)
    {
        // Assemble
        FileInfo f = TestFile(@".info\MessageAnalysis\Test", file);

        // Act
        Result<List<LeasedMessages>> result = CsvService.Parse<LeasedMessages>(f);
        List<IMessage> translation = result.IsSuccess
            ? result.Value.Select(c => c.Translate()).ToList()
            : throw new Exception(result.Error);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
        Assert.NotEmpty(translation);
    }
    #endregion

    #region TranslationProperlyRetrievesAndTranslates
    [
        Theory,
        InlineData(180),
        InlineData(181),
        InlineData(182),
        InlineData(183),
        InlineData(184),
        InlineData(185),
        InlineData(186),
        InlineData(187),
        InlineData(188),
        InlineData(189),
    ]
    public void RetrieveAndTranslateLeafThreads(int days)
    {
        // Assemble
        FileInfo json = TestFile(@".info\Testing", "DateReading.json");
        FileInfo csv = TestFile(@".info\Testing", "DateReading.csv");
        DateTimeOffset date1 = DateTime.Now - TimeSpan.FromDays(days + 0);
        DateTimeOffset date2 = DateTime.Now - TimeSpan.FromDays(days + 2);
        DateTimeOffset date3 = DateTime.Now - TimeSpan.FromDays(days + 3);
        DateTimeOffset date4 = DateTime.Now - TimeSpan.FromDays(days + 4);
        DateTimeOffset date5 = DateTime.Now - TimeSpan.FromDays(days + 5);
        DateTimeOffset date6 = DateTime.Now - TimeSpan.FromDays(days + 6);
        DateTimeOffset date7 = DateTime.Now - TimeSpan.FromDays(days + 7);
        DateTimeOffset date8 = DateTime.Now - TimeSpan.FromDays(days + 8);
        DateTimeOffset date9 = DateTime.Now - TimeSpan.FromDays(days + 9);
        List<DateTimeOffset> dates = [
            date1, date2, date3, date4, date5, date6, date7, date8, date9
        ];
        List<DateMap> datesMap = [
            new DateMap() { Date = date1 },
            new DateMap() { Date = date2 },
            new DateMap() { Date = date3 },
            new DateMap() { Date = date4 },
            new DateMap() { Date = date5 },
            new DateMap() { Date = date6 },
            new DateMap() { Date = date7 },
            new DateMap() { Date = date8 },
            new DateMap() { Date = date9 },
        ];

        // Act
        Result input = CsvService.Write<DateMap, DateMap>(csv, datesMap);
        Result<List<DateMap>> action = CsvService.Parse<DateMap>(csv);
        List<DateTimeOffset> actualDates = action.IsSuccess ? action.Value.Select(s => s.Date).ToList() : [];
        if (action.IsSuccess)
        {
            // Re-convert datetimeoffsets into datemaps
            IEnumerable<DateMap> datemapstosave = actualDates.Select(s => new DateMap() { Date = s });

            // Re-save information
            Result saved = CsvService.Write<DateMap, DateMap>(csv, datemapstosave);

            // Re-retrieve information
            Result<List<DateMap>> reaction = CsvService.Parse<DateMap>(csv);

            // Re-translate information
            actualDates = reaction.IsSuccess ? reaction.Value.Select(s => s.Date).ToList() : [];
        }

        // Assert
        Assert.True(action.IsSuccess);
        Assert.True(input.IsSuccess);
        foreach (DateTimeOffset date in dates)
        {
            var expected = new DateTimeOffset(new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second));
            Assert.Contains(expected, actualDates);
        }
    }
    #endregion

    #region Ensure that reading and writing to the list of items with a specific map does not change the data
    [
        Theory
        //Skip = ""
        ,
        InlineData(10),
        InlineData(11),
        InlineData(12),
        InlineData(13),
    ]
    public void ReadingandWritingDoesNotChangeDates(int times)
    {
        /************************************************************************
         * Assemble
         ************************************************************************/
        FileInfo file = TestFile(@".info\MessageAnalysis\Test", "TestReaderFile.csv");
        if (!file.Exists) File.Create(file.FullName);

        // Read the contents 
        Result<List<QualifiedMessageMap>> reader = CsvService.Parse<QualifiedMessageMap>(file);
        IEnumerable<QualifiedMessageRecord> expected = reader.Value.Select(c => c.Translate());

        /************************************************************************
         * Act
         ************************************************************************/
        // Read the contents from the file, then write them to the file repeatedly.
        for (int i = 0; i < times; i++)
        {
            // Read the contents 
            Result<List<QualifiedMessageMap>> contents = CsvService.Parse<QualifiedMessageMap>(file);
            IEnumerable<QualifiedMessageRecord> contentValue = contents.Value.Select(c => c.Translate());

            // Write the contents to file
            Result written = CsvService.Write<QualifiedMessageRecord, QualifiedMessageMap>(file, contentValue);
        }

        // Retrieve contents
        Result<List<QualifiedMessageMap>> read = CsvService.Parse<QualifiedMessageMap>(file);
        List<QualifiedMessageRecord> actual = read.Value.Select(c => c.Translate()).ToList();

        /************************************************************************
         * Assert
         ************************************************************************/
        actual.ForEach(
            act => Assertions(act, expected.First(expect => expect.Message.Number.Number == act.Message.Number.Number))
            );

        #region Local
        static void Assertions(QualifiedMessageRecord actual, QualifiedMessageRecord expected)
        {
            /************************************************************************
             * Message
             ************************************************************************/
            // Message Assertions
            Assert.Equal(expected.Message.Date, actual.Message.Date);

            // Other Message Assertions
            Assert.Equal(expected.Message.Contents, actual.Message.Contents);
            Assert.Equal(expected.Message.Source, actual.Message.Source);

            /************************************************************************
             * Customer
             ************************************************************************/
            // Customer Date Assertions
            Assert.Equal(expected.Customer.Date, actual.Customer.Date);
            Assert.Equal(expected.Customer.SubscriptionStartDate, actual.Customer.SubscriptionStartDate);
            Assert.Equal(expected.Customer.CustomerCancelDate, actual.Customer.CustomerCancelDate);
            Assert.Equal(expected.Customer.SubscriptionCancelDate, actual.Customer.SubscriptionCancelDate);

            // Other Customer Assertions
            Assert.Equal(expected.Customer.CustomerId, actual.Customer.CustomerId);
            Assert.Equal(expected.Customer.SubscriptionId, actual.Customer.SubscriptionId);
            Assert.Equal(expected.Customer.Number.Number, actual.Customer.Number.Number);
            Assert.Equal(expected.Customer.Number2.Number, actual.Customer.Number2.Number);
            Assert.Equal(expected.Customer.CustomerActive, actual.Customer.CustomerActive);
            Assert.Equal(expected.Customer.SubscriptionActive, actual.Customer.SubscriptionActive);
            Assert.Equal(expected.Customer.InitialCompleted, actual.Customer.InitialCompleted);
            Assert.Equal(expected.Customer.ContractValue, actual.Customer.ContractValue);
            Assert.Equal(expected.Customer.Sellers, actual.Customer.Sellers);

            /************************************************************************
             * QualifiedMessageRecord
             ************************************************************************/
            // Billability asssertions
            Assert.Equal(expected.Billable, actual.Billable);
            Assert.Equal(expected.IsSalesLead, actual.IsSalesLead);
        }
        #endregion
    }
    #endregion
}

#region Please do not move this to a new file
internal class DateMap : ClassMap<DateMap>
{
    private const string name = "Date";
    public DateMap()
    {
        Map(m => m.Date.DateTime).Index(0).Name(name);
    }
    [Name(name)]
    public DateTimeOffset Date { get; set; }
}
#endregion