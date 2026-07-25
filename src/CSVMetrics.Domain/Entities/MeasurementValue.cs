namespace CSVMetrics.Domain.Entities;

public class MeasurementValue
{
    public long Id { get; set; }
    public string FileName { get; set; } = default!;
    public DateTimeOffset Date { get; set; }
    public double ExecutionTime { get; set; }
    public double Value { get; set; }
}