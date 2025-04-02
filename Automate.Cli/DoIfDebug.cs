using Automate.Domain.SolutionFunctionality;
using System.Diagnostics;

namespace Automate.Cli;

/// <summary>
/// This implementation uses TWO different methods for finding whether the current build is in debug
/// </summary>
internal class DoIfDebug : IDetermineDebug
{
    /// <summary>
    /// If the build is in debug, returns true, otherwise, returns false
    /// </summary>
    /// <returns>A boolean which indicates whether the current build is in debug</returns>
    public bool IsDebug()
    {
#if DEBUG
        SetDebug();
#endif
        return debug;
    }

    [Conditional("DEBUG")]
    private void SetDebug()
    {
        debug = true;
    }
    private bool debug = false;
}
