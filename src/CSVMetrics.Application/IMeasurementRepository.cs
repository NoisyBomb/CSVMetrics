using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Application;

public interface IMeasurementRepository
{
    Task AddRangeAsync(List<MeasurementValue> values);
    Task DeleteByFileNameAsync(string fileName);
    Task<List<MeasurementValue>> GetRecentByFileNameAsync(string fileName, int count);
}