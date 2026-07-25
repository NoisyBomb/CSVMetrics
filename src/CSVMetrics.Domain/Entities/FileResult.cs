namespace CSVMetrics.Domain.Entities;

public class FileResult
{
    public long Id { get; set; }
    public string FileName { get; set; } = default!;
    public double TimeDeltaSeconds { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public double AvgExecutionTime { get; set; }
    public double AvgValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
    public DateTimeOffset ProcessedAt { get; set; }
}