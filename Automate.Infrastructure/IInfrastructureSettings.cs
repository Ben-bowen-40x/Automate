using Automate.Infrastructure.AngleService;
using Automate.Infrastructure.DatabaseService;
using Automate.Infrastructure.FatSap;
using Automate.Infrastructure.LeafClientService;

namespace Automate.Infrastructure;

public interface IInfrastructureSettings : IDwhSettings, ILeafApiSettings, ISharpQuerySettings, IFatSapSettings
{
    string? Cookie { get; set; }
    string? NoCookie { get; set; }
}
