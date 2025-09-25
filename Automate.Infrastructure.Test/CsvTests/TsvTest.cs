using Automate.Domain.SolutionFunctionality;
using Automate.Infrastructure.CsvManipulationService;
using Automate.Infrastructure.DataRetrievalFormats;

namespace Automate.Infrastructure.Test.CsvTests;

public class TsvTest
{
    [Fact]
    public void CsvService_CanReadTsvFiles()
    {
        // Assemble
        FileInfo loc = FolderFinder.GetLocalFile(nameof(Infrastructure), ".info/Reports/QueryReports", "LeafQueryOut.tsv");

        // Act
        var result = CsvService.Parse<MessageClass>(loc);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
