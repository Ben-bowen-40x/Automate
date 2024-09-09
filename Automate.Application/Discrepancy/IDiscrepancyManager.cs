namespace Automate.Application.Discrepancy;

public interface IDiscrepancyManager
{
    Dictionary<bool, FileInfo> ManageDiscrepancyAnalysis(string fileLocation, string reportFileLocation, string comparisonFileLocation);
}