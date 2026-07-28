using CSVMetrics.Application;
using CSVMetrics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSVMetrics.Infrastructure;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly AppDb _context;
    public MeasurementRepository(AppDb context)
    {
        _context = context;
    }
    public async Task AddRangeAsync(List<MeasurementValue> values)
    {
        await _context.Values.AddRangeAsync(values);
    }
    public async Task DeleteByFileNameAsync(string fileName)
    {
        var existing = await _context.Values.Where(v => v.FileName == fileName).ToListAsync();
        _context.Values.RemoveRange(existing);
    }
}