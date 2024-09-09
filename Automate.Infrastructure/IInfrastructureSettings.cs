using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.LeafClientService;

namespace Automate.Infrastructure;

public interface IInfrastructureSettings : IDwhSettings, ILeafApiSettings
{
}
