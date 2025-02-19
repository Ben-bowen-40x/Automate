using Automate.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Automate.Application.Discrepancy;

public interface ITypedDiscrepancyManager
{
    Result<FileInfo> Manage<T, TComparison>(FileInfo billedCalls, FileInfo comparisonCalls, FileInfo reportLoc) where T : IConvert where TComparison : IConvert;
}