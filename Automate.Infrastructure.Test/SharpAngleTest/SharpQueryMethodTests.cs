using Automate.Infrastructure.AngleService;
using Automate.Infrastructure.Test.TestConfigurations;
namespace Automate.Infrastructure.Test.CatmanTest;

public class SharpQueryMethodTests
{
    public readonly static IInfrastructureSettings _config = (IInfrastructureSettings)new InfraTestConfiguration().TestSettings;
    private readonly SharpQueryService _queryservice = new(_config);
    const string _undev = "This method has not been created and cannot be tested";
    [
        Fact
        (Skip = _undev)
    ]
    public void TestCatmanGuliagar()
    {
        var doit = _queryservice.SubmitForm_Guliagar();
        Assert.True(doit.IsSuccess);
    }
    [
        Fact
        (Skip = _undev)
    ]
    public async Task FormSubmits()
    {
        var doit = await _queryservice.Submit_Form_Guliagar();
        Assert.True(doit.IsSuccess);
    }
}
