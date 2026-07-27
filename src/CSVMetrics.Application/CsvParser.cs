using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVMetrics.Application;

public class CsvParser
{
    public List<CsvDto> Parse(Stream fileStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<CsvDto>().ToList();
    }
}