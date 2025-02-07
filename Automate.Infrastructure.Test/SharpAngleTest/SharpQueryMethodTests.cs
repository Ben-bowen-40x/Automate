using Automate.Infrastructure.AngleService;
using Automate.Infrastructure.Test.TestConfigurations;
namespace Automate.Infrastructure.Test.CatmanTest;

public class SharpQueryMethodTests
{
    public readonly static ISharpQueryTestSettings _config = new InfraTestConfiguration().TestSettings;
    private readonly SharpQueryService _queryservice = new(_config);

    [Fact]
    public void TestCatmanGuliagar()
    {
        var doit = _queryservice.SubmitForm_Guliagar();
        Assert.NotNull(doit);
    }
    [Fact]
    public async Task FormSubmits()
    {
        var doit = await _queryservice.Submit_Form_Guliagar();
        Assert.True(doit.IsSuccess);
    }
}
