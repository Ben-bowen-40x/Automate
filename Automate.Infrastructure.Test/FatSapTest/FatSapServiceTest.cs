using Automate.Infrastructure.FatSap;
using Automate.Infrastructure.Test.TestConfigurations;
using Automate.Application.InfrastructureValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.Test.FatSapTest;

public class FatSapServiceTest
{
    static readonly InfraTestConfiguration _config = new();
    static readonly IInfrastructureTestSettings _settings = _config.TestSettings;
    static readonly IHttpClientFactory _factory = _config.ClientFactory;

    [Fact]
    public void FatService_GetsCalls()
    {
        // Assemble
        FatSapService fatService = new(_settings, _factory);
        var now = DateTime.Now;

        // Act
        Task<Result<FatSapRoot>> actual = fatService.GetCallAsync(now - new TimeSpan(1, 0, 0, 0), now);
        actual.Wait();

        // Assert
        Assert.True(actual.IsCompletedSuccessfully);
        Assert.True(actual.Result.IsSuccess);
    }

    [Fact]
    public async Task FatService_PaginatesCalls()
    {
        // Assemble
        FatSapService fatService = new(_settings, _factory);
        var now = DateTime.Now;

        // Act
        Task<Result<FatSapRoot>[]> actual = fatService.GetCallsAsync<FatSapRoot>(now - new TimeSpan(1, 0, 0, 0), now);
        actual.Wait();

        // Assert
        Assert.True(actual.IsCompletedSuccessfully);
        actual.Result.ToList().ForEach(r => Assert.True(r.IsSuccess));
    }
}
