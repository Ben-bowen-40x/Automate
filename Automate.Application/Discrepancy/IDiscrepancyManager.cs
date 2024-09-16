using CSharpFunctionalExtensions;

namespace Automate.Application.Discrepancy;

public interface IDiscrepancyManager
{
    Result<FileInfo> ManageDiscrepancyAnalysis(string fileLocation, string reportFileLocation, string comparisonFileLocation);
}