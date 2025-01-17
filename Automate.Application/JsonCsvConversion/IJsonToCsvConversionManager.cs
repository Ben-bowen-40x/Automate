using CSharpFunctionalExtensions;
using CsvHelper.Configuration;

namespace Automate.Application.JsonCsvConversion
{
    public interface IJsonToCsvConversionManager
    {
        Result<FileInfo> ManageConversion<T, TMap>(FileInfo jsonFile, FileInfo csvFile) where TMap : ClassMap<T>;
    }
}