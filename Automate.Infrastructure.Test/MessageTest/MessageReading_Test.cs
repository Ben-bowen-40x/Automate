using Automate.Domain.SolutionFunctionality;
using Automate.Domain.ValueObjects;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Translation.MessageTranslate;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.Test.MessageTest;

public class MessageReading_Test
{
    private FileInfo TestFile(string file) => FolderFinder.GetLocalFile(nameof(Infrastructure), @".info\MessageAnalysis\Test", file);
    [
        Theory,
        // Skip = ""
        InlineData("LeasedMessagesTest.csv"),
    ]
    public void SpecialMessageType_CanBeReadFromTest(string file)
    {
        // Assemble
        FileInfo f = TestFile(file);

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
}
