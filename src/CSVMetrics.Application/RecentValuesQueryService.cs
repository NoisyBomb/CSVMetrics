using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Application;

public class RecentValuesQueryService
{
    private readonly IMeasurementRepository _measurementRepository;
    public RecentValuesQueryService(IMeasurementRepository measurementRepository)
    {
        _measurementRepository = measurementRepository;
    }
    public async Task<List<MeasurementValue>> GetRecentValuesAsync(string fileName)
    {
        return await _measurementRepository.GetRecentByFileNameAsync(fileName, 10);
    }
}