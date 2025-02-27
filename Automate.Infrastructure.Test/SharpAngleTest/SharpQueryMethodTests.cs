using Automate.Infrastructure.AngleService;
using Automate.Infrastructure.Test.TestConfigurations;
namespace Automate.Infrastructure.Test.CatmanTest;

public class SharpQueryMethodTests
{
    public readonly static IInfrastructureSettings _config = (IInfrastructureSettings)new InfraTestConfiguration().TestSettings;
    private readonly SharpQueryService _queryservice = new(_config);

    [
        Fact
        (Skip = "This has not been created yet")
    ]
    public void TestCatmanGuliagar()
    {
        var doit = _queryservice.SubmitForm_Guliagar();
        Assert.True(doit.IsSuccess);
    }
    [
        Fact
        (Skip = "This has not been created yet")
    ]
    public async Task FormSubmits()
    {
        var doit = await _queryservice.Submit_Form_Guliagar();
        Assert.True(doit.IsSuccess);
    }
}
