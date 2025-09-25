using CSharpFunctionalExtensions;

namespace Automate.Application.LeafExclusion
{
    public interface ILeafExclusionManager
    {
        Result Manage(FileInfo leafRepo, FileInfo output);
    }
}