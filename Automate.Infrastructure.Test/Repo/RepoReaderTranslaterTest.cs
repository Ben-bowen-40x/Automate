using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.DwhRepoUpdateService;
using CSharpFunctionalExtensions;
using NSubstitute;

namespace Automate.Infrastructure.Test.Repo;

public class RepoReaderTranslaterTest
{
    private readonly IDwhSettings mockSettings = Substitute.For<IDwhSettings>();
    private DwhRepoService Service => new(mockSettings);
    [
        Theory,
        InlineData(10),
        InlineData(9),
        InlineData(8),
        InlineData(7),
        InlineData(6),
        InlineData(5),
    ]
    public void CustSubReader_TranslatesProperly(int reps)
    {
        // Assemble
        // Get the test file
        FileInfo file = Functions.TestFile(@".info/ApiRepos/RepoTests", "CustomerRepoTest.json");

        // Get the test file Contents
        Result<List<CustSubJsonReader>> contents = Service.GetRepo<CustSubJsonReader>(file.FullName);
        List<CustSubJsonReader> expected = contents.IsSuccess
            ? contents.Value
            : throw new Exception("Failure retrieving expected values from repo");

        // Act
        // Write the test file contents repeatedly
        for (int i = 0; i < reps; i++)
        {
            Result<List<CustSubJsonReader>> inter = Service.GetRepo<CustSubJsonReader>(file.FullName);
            List<CustSubJsonReader> list = inter.IsSuccess
                ? inter.Value
                : throw new Exception($"Failure retrieving values from repo during iterative execution. Iteration index: {i}");
            Result u = Service.Update(list, file.FullName);
            if (u.IsFailure) throw new Exception("Failure updating repo.");
        }

        // Assert
        Result<List<CustSubJsonReader>> c = Service.GetRepo<CustSubJsonReader>(file.FullName);
        List<CustSubJsonReader> actual = c.IsSuccess
            ? contents.Value
            : throw new Exception("Failure retrieving expected values from repo");

        expected.ForEach(e =>
        {
            long sid = e.SubscriptionId;
            List<CustSubJsonReader> actualSid = actual
                .Where(x => x.SubscriptionId == sid)
                .ToList();
            Assert.NotEmpty(actualSid);
            Assert.Equal(sid, actualSid[0].SubscriptionId);
        });
    }
}
