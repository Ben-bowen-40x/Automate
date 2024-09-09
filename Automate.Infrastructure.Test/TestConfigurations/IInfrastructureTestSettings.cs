using Automate.Infrastructure.Test.CsvTests;
using Automate.Infrastructure.Test.LeafApiTests;

namespace Automate.Infrastructure.Test.TestConfigurations;

public interface IInfrastructureTestSettings : ILeafTestSettings, ICsvTestFileSettings
{

}
