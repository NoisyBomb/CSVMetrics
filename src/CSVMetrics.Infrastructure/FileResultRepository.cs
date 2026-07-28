using CSVMetrics.Application;
using CSVMetrics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSVMetrics.Infrastructure;

public class FileResultRepository : IFileResultRepository
{
    private readonly AppDb _context;
    public FileResultRepository(AppDb context)
    {
        _context = context;
    }
    public async Task<FileResult?> GetByFileNameAsync(string fileName)
    {
        return await _context.Results.FirstOrDefaultAsync(r => r.FileName == fileName);
    }
    public async Task AddAsync(FileResult result)
    {
        await _context.Results.AddAsync(result);
    }
    public async Task DeleteAsync(FileResult result)
    {
        _context.Results.Remove(result);
        await Task.CompletedTask;
    }
    public async Task<List<FileResult>> GetByFilterAsync(ResultsFilterDto filter)
    {
        var query = _context.Results.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.FileName))
        {
            query = query.Where(r => r.FileName == filter.FileName);
        }
        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(r => r.StartDate >= filter.StartDateFrom.Value);
        }
        if (filter.StartDateTo.HasValue)
        {
            query = query.Where(r => r.StartDate <= filter.StartDateTo.Value);
        }
        if (filter.AvgValueFrom.HasValue)
        {
            query = query.Where(r => r.AvgValue >= filter.AvgValueFrom.Value);
        }
        if (filter.AvgValueTo.HasValue)
        {
            query = query.Where(r => r.AvgValue <= filter.AvgValueTo.Value);
        }
        if (filter.AvgExecutionTimeFrom.HasValue)
        {
            query = query.Where(r => r.AvgExecutionTime >= filter.AvgExecutionTimeFrom.Value);
        }
        if (filter.AvgExecutionTimeTo.HasValue)
        {
            query = query.Where(r => r.AvgExecutionTime <= filter.AvgExecutionTimeTo.Value);
        }
        return await query.ToListAsync();
    }
}