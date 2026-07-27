using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Application;

public class AggregateCalculator
{
    public FileResult Calculate(List<CsvDto> rows, string fileName)
    {
        var dates = rows.Select(r => r.Date).ToList();
        var executionTimes = rows.Select(r => r.ExecutionTime).ToList();
        var values = rows.Select(r => r.Value).OrderBy(v => v).ToList();
        var minDate = dates.Min();
        var maxDate = dates.Max();
        return new FileResult
        {
            FileName = fileName,
            StartDate = minDate,
            TimeDeltaSeconds = (maxDate - minDate).TotalSeconds,
            AvgExecutionTime = executionTimes.Average(),
            AvgValue = values.Average(),
            MedianValue = CalculateMedian(values),
            MaxValue = values.Max(),
            MinValue = values.Min(),
            ProcessedAt = DateTimeOffset.UtcNow
        };
    }

    private static double CalculateMedian(List<double> sortedValues)
    {
        int count = sortedValues.Count;
        int middle = count / 2;
        if (count % 2 == 0)
        {
            return (sortedValues[middle - 1] + sortedValues[middle]) / 2.0;
        }
        return sortedValues[middle];
    }
}