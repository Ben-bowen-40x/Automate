namespace Automate.Infrastructure.Test.LeafApiServiceTests;

public class LeafApiServiceTest
{
    [
        Theory,
        InlineData(true),
        InlineData(false),
    ]
    public void EnsureThatIdempotencyIsMaintainedFromOldRepoToNewRepo(bool historicalMatch) { }
}
