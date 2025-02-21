using Automate.Infrastructure.Test.CatmanTest;
using Automate.Infrastructure.Test.CsvTests;
using Automate.Infrastructure.Test.DiscrepancyTest;
using Automate.Infrastructure.Test.LeafApiTests;

namespace Automate.Infrastructure.Test.TestConfigurations;

public interface IInfrastructureTestSettings : ILeafTestSettings, ICsvTestFileSettings, ISharpQueryTestSettings, IDwhTestSettings { }
