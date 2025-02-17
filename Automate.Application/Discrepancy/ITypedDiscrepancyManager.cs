using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.Discrepancy;

public interface ITypedDiscrepancyManager
{
    Result<FileInfo> Manage<T>(FileInfo billedCalls, FileInfo comparisonCalls, string reportLoc) where T : IConvert;
}