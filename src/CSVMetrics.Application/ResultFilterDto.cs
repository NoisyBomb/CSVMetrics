namespace CSVMetrics.Application;

public class ResultsFilterDto
{
    public string? FileName { get; set; }
    public DateTimeOffset? StartDateFrom { get; set; }
    public DateTimeOffset? StartDateTo { get; set; }
    public double? AvgValueFrom { get; set; }
    public double? AvgValueTo { get; set; }
    public double? AvgExecutionTimeFrom { get; set; }
    public double? AvgExecutionTimeTo { get; set; }
}